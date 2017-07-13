using gt.ai;
using UnityEngine;

namespace gt.FlappyBird
{
    public class Birds : MonoBehaviour
    {
        public AiController AiController;
        public GameObject BirdPrefab;
        public int Alive;
        public int All;
        public Pipes Pipes;
        public float MaxScore;
        public int Generation;
        public float Score;
        private BirdBrain[] _birdBrains;

        private void Start()
        {
            var birds = AiController.Genomes;
            _birdBrains = new BirdBrain[birds];
            for (int i = 0; i < birds; i++)
            {
                var go = Instantiate(BirdPrefab, transform.position, Quaternion.identity, transform);
                var bird = go.GetComponent<Bird>();
                bird.Brain = new BirdBrain(i, AiController, bird, this);
                bird.Brain.OnBirdDie += OnOnDie;
                _birdBrains[i] = bird.Brain;
            }
            Alive = birds;
            All = Alive;
        }

        private void FixedUpdate()
        {
            Score += Time.fixedDeltaTime;
        }

        private void OnOnDie(BirdBrain brain)
        {
            Alive--;
            brain.OnBirdDie -= OnOnDie;
            brain.Score = Score;
            if (Alive == 0)
            {
                float[] scores = new float[_birdBrains.Length];
                float max = 0.0f;
                for (int i = 0; i < _birdBrains.Length; i++)
                {
                    var birdBrain = _birdBrains[i];
                    scores[i] = birdBrain.Score;

                    if (birdBrain.Score > MaxScore)
                    {
                        MaxScore = birdBrain.Score;
                    }

                    if (birdBrain.Score > max)
                    {
                        max = birdBrain.Score;
                    }
                }

                Generation++;

                AiController.NextGeneration(scores);

                Pipes.Restart();
                Start();

                Score = 0.0f;
            }
        }

        public void Restart()
        {
            for (int i = 0; i < _birdBrains.Length; i++)
            {
                var birdBrain = _birdBrains[i];
                birdBrain.Kill();
            }
            
            Pipes.Restart();

            Start();

            Score = 0.0f;
        }
    }
}