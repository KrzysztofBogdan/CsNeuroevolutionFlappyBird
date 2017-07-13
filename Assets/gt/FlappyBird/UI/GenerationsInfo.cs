using UnityEngine;

namespace gt.FlappyBird.UI
{
    public class GenerationsInfo : MonoBehaviour
    {
        public Birds Birds;

        private void OnGUI()
        {
            GUI.Box(new Rect(25, 5, 235, 100), "");
            
            GUI.Label(new Rect(30, 40, 500, 30), "Generation: " + Birds.Generation);
            GUI.Label(new Rect(30, 60, 500, 30), "Alive: " + Birds.Alive + "/" + Birds.All);

            long max = (long) (Birds.MaxScore * 100.0f);
            long current = (long) (Birds.Score * 100.0f);
            GUI.Label(new Rect(30, 80, 500, 30), "Max Score: " + max + " | Current Score: " + current);
        }
    }
}