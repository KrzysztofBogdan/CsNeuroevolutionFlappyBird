namespace gt.FlappyBird
{
    public class SimpleMath
    {
        public static float NormalizePoint(float min, float max, float value)
        {
            return PointOnLinearFunction(0.0f, min, 1.0f, max, value);
        }

        public static float PointOnLinearFunction(float x1, float y1, float x2, float y2, float x)
        {
            float slope = (y1 - y2) / (x1 - x2);
            float b = y1 - slope * x1;
            return (x - b) / slope;
        }
    }
}