using System;
using UnityEngine;

namespace gt.FlappyBird
{
    public class Bird : MonoBehaviour
    {
        public Rigidbody2D Body;
        public BirdBrain Brain;

        private void Start()
        {
            if (Brain == null)
            {
                Destroy(gameObject);
            }
        }

        private void FixedUpdate()
        {
            if (Brain.Compute(transform.position.y) > 0.5f)
            {
                Body.velocity = new Vector2(0.0f, 3.0f);
            }

            if (Body.velocity.y > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                float angle = Mathf.Lerp(0, 90.0f, -Body.velocity.y / 5);
                transform.rotation = Quaternion.Euler(0, 0, -angle);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            Brain.NotifyDie();
            Destroy(gameObject);
        }
    }
}