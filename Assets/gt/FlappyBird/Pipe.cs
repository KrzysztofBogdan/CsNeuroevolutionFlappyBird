using UnityEngine;

namespace gt.FlappyBird
{
    public class Pipe : MonoBehaviour
    {
        public float Speed = 1.0f;

        private void Update()
        {
            var position = transform.position;
            transform.position = new Vector3(position.x - (Time.deltaTime * Speed), position.y, position.z);
        }
    }
}