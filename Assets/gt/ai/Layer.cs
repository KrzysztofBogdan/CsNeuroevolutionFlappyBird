namespace gt.ai
{
    internal struct Layer
    {
        public Neuron[] Neurons;

        public override string ToString()
        {
            string[] neurons = new string[Neurons.Length];
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i] = Neurons[i].ToString();
            }
            return "Layer: " + string.Join(" | ", neurons);
        }
    }
}