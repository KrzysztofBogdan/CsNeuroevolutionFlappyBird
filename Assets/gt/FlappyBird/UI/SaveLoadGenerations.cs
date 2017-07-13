using gt.ai;
using UnityEngine;

namespace gt.FlappyBird.UI
{
    public class SaveLoadGenerations : MonoBehaviour
    {
        public Birds Birds;
        public AiController AiController;

        private byte[] _state = new byte[0];

        private void OnGUI()
        {
            GUI.enabled = Birds.Generation > 0;
            if (GUI.Button(new Rect(30, 110, 200, 30), "Save previous state"))
            {
                _state = AiController.SavePreviousGeneration();
            }

            GUI.enabled = _state.Length > 0;
            
            if (GUI.Button(new Rect(30, 150, 200, 30), "Load saved state"))
            {
                AiController.Load(_state);
                Birds.Restart();
            }

            GUI.enabled = true;
        }
    }
}