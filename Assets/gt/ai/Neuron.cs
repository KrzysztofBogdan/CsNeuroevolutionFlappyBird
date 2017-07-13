namespace gt.ai
{
    internal struct Neuron
    {
        public float Value;
        public float[] Weights;

        public override string ToString()
        {
            string[] weights = new string[Weights.Length];
            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = Weights[i].ToString("0.000");
            }
            return "" + Value + "[" + string.Join(",", weights) + "]";
        }
    }
}