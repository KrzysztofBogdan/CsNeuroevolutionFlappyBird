using UnityEngine;

namespace gt.FlappyBird
{
    public class TimeChange : MonoBehaviour
    {
        public float TimeScale = 1.0f;

        // Update is called once per frame
        void Update()
        {
            Time.timeScale = TimeScale;
        }
    }
}