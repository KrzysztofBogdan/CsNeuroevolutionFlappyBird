using UnityEngine;

namespace gt.FlappyBird.UI
{
    public class TimeToggle : MonoBehaviour
    {
        public TimeChange TimeChange;

        private bool _one = true;
        private bool _two;
        private bool _three;
        private bool _five;
        private bool _ten;

        private void OnGUI()
        {
            var prevContentColor = GUI.contentColor;
            GUI.Label(new Rect(30, 10, 100, 30), "Speed: ");
            float padding = 50.0f;
            var oneNow = GUI.Toggle(new Rect(padding + 30, 10, 30, 30), _one, "1");
            if (oneNow && !_one)
            {
                _two = _three = _five = _ten = false;
                _one = true;
                TimeChange.TimeScale = 1.0f;
            }

            var twoNow = GUI.Toggle(new Rect(padding + 60, 10, 30, 30), _two, "2");
            if (twoNow && !_two)
            {
                _one = _three = _five = _ten = false;
                _two = true;
                TimeChange.TimeScale = 2.0f;
            }

            var threeNow = GUI.Toggle(new Rect(padding + 90, 10, 30, 30), _three, "3");
            if (threeNow && !_three)
            {
                _one = _two = _five = _ten = false;
                _three = true;
                TimeChange.TimeScale = 3.0f;
            }

            var fiveNow = GUI.Toggle(new Rect(padding + 120, 10, 30, 30), _five, "5");
            if (fiveNow && !_five)
            {
                _one = _two = _three = _ten = false;
                _five = true;
                TimeChange.TimeScale = 5.0f;
            }

            var tenNow = GUI.Toggle(new Rect(padding + 150, 10, 30, 30), _ten, "10");
            if (tenNow && !_ten)
            {
                _one = _two = _three = _five = false;
                _ten = true;
                TimeChange.TimeScale = 10.0f;
            }
        }
    }
}