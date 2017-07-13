using System;
using System.Collections.Generic;
using System.IO;
using Random = UnityEngine.Random;

namespace gt.ai
{
    public class NeuroevolutionGeneration : IGeneration
    {
        private readonly int _inputs;
        private readonly int[] _hiddenLayers;
        private readonly int _outputs;
        private readonly int _genomes;
        private readonly int _elitism;
        private readonly int _freshGenomes;
        private readonly int _breedChilds;
        private readonly float _mutationRate;
        private readonly float _mutationRange;
        private readonly Generation _generation;
        private readonly Func<float, float> _activationFunction;
        private readonly IComparer<float> _floatDescending = new FloatDescendingComparer();
        private readonly IComparer<Genome> _genomeDescending = new GenomeDescendingComparer();

        private NeuroevolutionGeneration(int inputs, int[] hiddenLayers, int outputs, int genomes, int elitism,
            int freshGenomes, int breedChilds, float mutationRate, float mutationRange,
            Func<float, float> activationFunction, Generation newGeneration)
        {
            _inputs = inputs;
            _hiddenLayers = hiddenLayers;
            _outputs = outputs;
            _genomes = genomes;
            _elitism = elitism;
            _freshGenomes = freshGenomes;
            _breedChilds = breedChilds;
            _mutationRate = mutationRate;
            _mutationRange = mutationRange;
            _activationFunction = activationFunction;
            _generation = newGeneration;
        }

        public float[] Compute(int genomeNumber, float[] input)
        {
            var genome = _generation.Genomes[genomeNumber];

            var network = genome.Network;

            var layers = network.Layers;

            var inputLayer = layers[0];
            for (int i = 0; i < inputLayer.Neurons.Length; i++)
            {
                inputLayer.Neurons[i].Value = input[i];
            }

            var previousLayer = inputLayer;
            for (int i = 1; i < layers.Length; i++)
            {
                var layer = layers[i];

                for (int j = 0; j < layer.Neurons.Length; j++)
                {
                    var neuron = layer.Neurons[j];

                    float sum = 0;

                    var previousLayerNeurons = previousLayer.Neurons;
                    for (int k = 0; k < previousLayerNeurons.Length; k++)
                    {
                        var previousLayerNeuron = previousLayerNeurons[k];
                        sum += previousLayerNeuron.Value * neuron.Weights[k];
                    }

                    neuron.Value = _activationFunction(sum);
                    layer.Neurons[j] = neuron;
                }

                previousLayer = layer;
            }

            var outputLayerNeurons = layers[layers.Length - 1].Neurons;
            var result = new float[outputLayerNeurons.Length];
            for (int i = 0; i < outputLayerNeurons.Length; i++)
            {
                result[i] = outputLayerNeurons[i].Value;
            }
            return result;
        }

        public IGeneration NextGeneration(float[] scores)
        {
            int[] rank = new int[scores.Length];
            for (int i = 0; i < scores.Length; i++)
            {
                rank[i] = i;
            }

            Array.Sort(scores, rank, _floatDescending);

            var generation = _generation;
            var genomes = generation.Genomes;
            for (int i = 0; i < genomes.Length; i++)
            {
                var genome = genomes[i];
                genome.Score = scores[i];
                generation.Genomes[i] = genome;
            }

            var nextGenomes = new Genome[_genomes];

            // Keep best genomes
            for (int i = 0; i < _elitism; i++)
            {
                nextGenomes[i] = genomes[rank[i]];
            }

            // Add fresh genomes
            int start = _elitism;
            int end = start + _freshGenomes;
            for (int i = start; i < end; i++)
            {
                nextGenomes[i] = NewGenome(_inputs, _hiddenLayers, _outputs);
            }

            int position = end;
            end = _genomes;

            int max = 0;
            while (true)
            {
                for (int j = 0; j < max; j++)
                {
                    var childs = Breed(genomes[j], genomes[max], _breedChilds);

                    for (int i = 0; i < childs.Length; i++)
                    {
                        nextGenomes[position] = childs[i];
                        position++;
                        if (position == end)
                        {
                            goto finish;
                        }
                    }
                }

                max++;

                if (max == _genomes - 1)
                {
                    max = 0;
                }
            }

            finish:
            return new NeuroevolutionGeneration(
                _inputs,
                _hiddenLayers,
                _outputs,
                _genomes,
                _elitism,
                _freshGenomes,
                _breedChilds,
                _mutationRate,
                _mutationRange,
                _activationFunction,
                new Generation {Genomes = nextGenomes});
        }

        private Genome[] Breed(Genome first, Genome second, int childCound)
        {
            var childs = new Genome[childCound];

            for (int i = 0; i < childCound; i++)
            {
                var firstNetwork = first.Network;
                var secondNetwork = second.Network;

                var network = new Network();
                network.Layers = new Layer[firstNetwork.Layers.Length];

                for (int j = 0; j < network.Layers.Length; j++)
                {
                    var firstNetworkNeurons = firstNetwork.Layers[j].Neurons;
                    var secondNetworkNeurons = secondNetwork.Layers[j].Neurons;

                    var neurons = new Neuron[firstNetworkNeurons.Length];

                    for (int k = 0; k < neurons.Length; k++)
                    {
                        var firstWeights = firstNetworkNeurons[k].Weights;
                        var secondWeights = secondNetworkNeurons[k].Weights;
                        var weights = new float[firstWeights.Length];

                        for (int l = 0; l < firstWeights.Length; l++)
                        {
                            float weight;
                            if (Random.Range(0.0f, 1f) > 0.5f)
                            {
                                weight = firstWeights[l];
                            }
                            else
                            {
                                weight = secondWeights[l];
                            }

                            if (Random.Range(0.0f, 1f) > _mutationRate)
                            {
                                weight += Random.Range(-1.0f, 1.0f) * _mutationRange;
                            }

                            weights[l] = weight;
                        }
                        neurons[k] = new Neuron {Weights = weights};
                    }

                    network.Layers[j] = new Layer {Neurons = neurons};
                }

                childs[i] = new Genome {Network = network};
            }

            return childs;
        }

        #region Serialization and deserialization

        public byte[] Save()
        {
            using (var ms = new MemoryStream())
            {
                using (var w = new BinaryWriter(ms))
                {
                    var generation = _generation;
                    var genomes = generation.Genomes;
                    Array.Sort(genomes, _genomeDescending);
                    w.Write(genomes.Length);
                    foreach (var genome in genomes)
                    {
                        var network = genome.Network;
                        var layers = network.Layers;
                        w.Write(layers.Length);
                        foreach (var layer in layers)
                        {
                            var neurons = layer.Neurons;
                            w.Write(neurons.Length);
                            foreach (var neuron in neurons)
                            {
                                var weights = neuron.Weights;
                                w.Write(weights.Length);
                                foreach (var weight in weights)
                                {
                                    w.Write(weight);
                                }
                            }
                        }
                    }
                    w.Write(_inputs);
                    w.Write(_outputs);
                    w.Write(_genomes);
                    w.Write(_elitism);
                    w.Write(_freshGenomes);
                    w.Write(_breedChilds);
                    w.Write(_mutationRate);
                    w.Write(_mutationRange);

                    w.Write(_hiddenLayers.Length);
                    foreach (int hiddenLayer in _hiddenLayers)
                    {
                        w.Write(hiddenLayer);
                    }

                    w.Flush();
                    return ms.ToArray();
                }
            }
        }

        public static IGeneration Load(byte[] data, Func<float, float> activationFunction)
        {
            var generation = new Generation();
            using (var ms = new MemoryStream(data))
            {
                using (var r = new BinaryReader(ms))
                {
                    var genomesCount = r.ReadInt32();
                    generation.Genomes = new Genome[genomesCount];

                    for (int i = 0; i < genomesCount; i++)
                    {
                        var layersCount = r.ReadInt32();
                        var genome = new Genome();
                        var network = new Network();
                        network.Layers = new Layer[layersCount];

                        for (int j = 0; j < layersCount; j++)
                        {
                            var neuronsCount = r.ReadInt32();
                            var layer = new Layer();
                            layer.Neurons = new Neuron[neuronsCount];

                            for (int k = 0; k < neuronsCount; k++)
                            {
                                var weightsCount = r.ReadInt32();
                                var neuron = new Neuron();
                                neuron.Weights = new float[weightsCount];

                                for (int l = 0; l < weightsCount; l++)
                                {
                                    neuron.Weights[l] = r.ReadSingle();
                                }

                                layer.Neurons[k] = neuron;
                            }

                            network.Layers[j] = layer;
                        }

                        genome.Network = network;
                        generation.Genomes[i] = genome;
                    }

                    int inputs = r.ReadInt32();
                    int outputs = r.ReadInt32();
                    int genomes = r.ReadInt32();
                    int elitism = r.ReadInt32();
                    int freshGenomes = r.ReadInt32();
                    int breedChilds = r.ReadInt32();
                    float mutationRate = r.ReadSingle();
                    float mutationRange = r.ReadSingle();

                    int hiddenLayersCount = r.ReadInt32();
                    int[] hiddenLayers = new int[hiddenLayersCount];
                    for (int i = 0; i < hiddenLayersCount; i++)
                    {
                        hiddenLayers[i] = r.ReadInt32();
                    }

                    return new NeuroevolutionGeneration(
                        inputs,
                        hiddenLayers,
                        outputs,
                        genomes,
                        elitism,
                        freshGenomes,
                        breedChilds,
                        mutationRate,
                        mutationRange,
                        activationFunction,
                        generation);
                }
            }
        }

        #endregion

        #region Process Of Creation

        private static Generation NewGeneration(int inputs, int[] hiddenLayers, int outputs, int genomes)
        {
            var newGeneration = new Generation();
            newGeneration.Genomes = new Genome[genomes];

            for (int i = 0; i < newGeneration.Genomes.Length; i++)
            {
                newGeneration.Genomes[i] = NewGenome(inputs, hiddenLayers, outputs);
            }

            return newGeneration;
        }

        private static Genome NewGenome(int inputs, int[] hiddenLayers, int outputs)
        {
            var newGeome = new Genome();
            newGeome.Network = new Network();
            newGeome.Network.Layers = new Layer[2 + hiddenLayers.Length];
            newGeome.Network.Layers[0] = NewLayer(inputs, 0);

            var previousNeuronsCount = inputs;
            for (int i = 0; i < hiddenLayers.Length; i++)
            {
                int neuronsCount = hiddenLayers[i];
                newGeome.Network.Layers[i + 1] = NewLayer(neuronsCount, previousNeuronsCount);
                previousNeuronsCount = neuronsCount;
            }

            newGeome.Network.Layers[newGeome.Network.Layers.Length - 1] = NewLayer(outputs, previousNeuronsCount);

            return newGeome;
        }

        private static Layer NewLayer(int neuronsCount, int weights)
        {
            var newLayer = new Layer();
            newLayer.Neurons = new Neuron[neuronsCount];
            for (int i = 0; i < newLayer.Neurons.Length; i++)
            {
                newLayer.Neurons[i] = NewNeuron(weights);
            }
            return newLayer;
        }

        private static Neuron NewNeuron(int weights)
        {
            var newNeuron = new Neuron();
            newNeuron.Weights = new float[weights];
            for (int i = 0; i < newNeuron.Weights.Length; i++)
            {
                newNeuron.Weights[i] = RandomClamped();
            }
            return newNeuron;
        }

        private static float RandomClamped()
        {
            return Random.Range(-1.0f, 1.0f);
        }

        #endregion

        #region Instantiation

        /// <summary>
        /// Parematers are explained in <see cref="AiController"/>
        /// </summary>
        public static IGeneration Create(
            int inputs,
            int[] hiddenLayers,
            int outputs,
            int genomes,
            int elitism,
            int freshGenomes,
            int breedChilds,
            float mutationRate,
            float mutationRange,
            Func<float, float> activationFunction)
        {
            var evolution = new NeuroevolutionGeneration(inputs,
                hiddenLayers,
                outputs,
                genomes,
                elitism,
                freshGenomes,
                breedChilds,
                mutationRate,
                mutationRange,
                activationFunction,
                NewGeneration(inputs, hiddenLayers, outputs, genomes));
            return evolution;
        }

        #endregion

        private class FloatDescendingComparer : IComparer<float>
        {
            public int Compare(float x, float y)
            {
                return y.CompareTo(x);
            }
        }

        private class GenomeDescendingComparer : IComparer<Genome>
        {
            public int Compare(Genome x, Genome y)
            {
                return y.Score.CompareTo(x.Score);
            }
        }
    }
}