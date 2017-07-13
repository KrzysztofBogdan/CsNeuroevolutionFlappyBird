using UnityEngine;

namespace gt.FlappyBird
{
    public class Parallax : MonoBehaviour
    {
        public Renderer Renderer;
        public float Speed = 0.5f;
        public float Start = 0;

        Vector2 _offset = new Vector2(0, 0);

        private void Update()
        {
            _offset.x = Start + Time.time * Speed;

            Renderer.material.mainTextureOffset = _offset;
        }
    }
}