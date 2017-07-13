using gt.ai;
using UnityEngine;

namespace gt.FlappyBird
{
    public delegate void BirdDie(BirdBrain brain);

    public class BirdBrain
    {
        public float Score { get; set; }
        public event BirdDie OnBirdDie = delegate { };

        private readonly int _genome;
        private readonly AiController _aiController;
        private readonly Bird _bird;
        private readonly Birds _birds;

        public BirdBrain(int genome, AiController aiController, Bird bird, Birds birds)
        {
            _genome = genome;
            _aiController = aiController;
            _bird = bird;
            _birds = birds;
            Score = 0.0f;
        }

        public float Compute(float position)
        {
            var pipes = _birds.Pipes;
            var pipePosition = pipes.NextPipe;
            position = SimpleMath.NormalizePoint(pipes.MinHeight, pipes.MaxHeight, position);
            return _aiController.Compute(_genome, new[] {position, pipePosition})[0];
        }

        public void Kill()
        {
            if (_bird != null)
            {
                Object.Destroy(_bird.gameObject);
            }
        }

        public void NotifyDie()
        {
            OnBirdDie(this);
        }
    }
}