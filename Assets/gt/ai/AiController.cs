using System;
using UnityEngine;

namespace gt.ai
{
    public class AiController : MonoBehaviour
    {
        [Tooltip("Nuber of inputs.")] public int Inputs;

        [Tooltip("Size of list defines nuber of hidden layers. Value defines numer of neurons in hidden layer.")]
        public int[] HiddenLayers;

        [Tooltip("Number of outputs")] public int Outputs;
        [Tooltip("Number of genomes")] public int Genomes;
        [Tooltip("This number defines how many genomes will enter new generation")] public int Elitism;
        [Tooltip("How many new (with random weights) genomes will enter new generaion")] public int FreshGenomes;

        [Tooltip("How many childs will be generated from pair of best genomes. Pairs from 'elite' genomes")]
        public int BreedChilds;

        [Range(0.0f, 1.0f)] [Tooltip("Percantege chance for child to mutate. 0 -> 0% and 1 -> 100%")]
        public float MutationRate;

        [Tooltip("How much mutation can affect weight (-MutationRange, MutationRange)")] public float MutationRange;

        [Tooltip("Acivation function, more on https://en.wikipedia.org/wiki/Activation_function")]
        public ActivationFunctionType ActivationFunctionType = ActivationFunctionType.SoftStep;

        public IGeneration Generation;
        public IGeneration PreviousGeneration;

        private void Start()
        {
            Generation = NeuroevolutionGeneration.Create(
                Inputs,
                HiddenLayers,
                Outputs,
                Genomes,
                Elitism,
                FreshGenomes,
                BreedChilds,
                MutationRate,
                MutationRange,
                ActivationFunction.FromType(ActivationFunctionType));
        }

        public float[] Compute(int genome, float[] input)
        {
            return Generation.Compute(genome, input);
        }

        public void NextGeneration(float[] score)
        {
            PreviousGeneration = Generation;
            Generation = Generation.NextGeneration(score);
        }

        public byte[] SavePreviousGeneration()
        {
            return PreviousGeneration.Save();
        }

        public void Load(byte[] bytes)
        {
            Generation = NeuroevolutionGeneration.Load(bytes,
                ActivationFunction.FromType(ActivationFunctionType));
        }
    }

    public static class ActivationFunction
    {
        public static Func<float, float> FromType(ActivationFunctionType activationFunctionType)
        {
            switch (activationFunctionType)
            {
                case ActivationFunctionType.Identity:
                    return Identity;
                case ActivationFunctionType.BinaryStep:
                    return BinaryStep;
                case ActivationFunctionType.SoftStep:
                    return SoftStep;
                case ActivationFunctionType.TanH:
                    return TanH;
                case ActivationFunctionType.ArcTan:
                    return ArcTan;
                case ActivationFunctionType.SoftSign:
                    return SoftSign;
                default:
                    throw new ArgumentOutOfRangeException("activationFunctionType", activationFunctionType, null);
            }
        }

        private static float Identity(float value)
        {
            return value;
        }

        private static float BinaryStep(float value)
        {
            return value < 0.0f ? 0.0f : 1.0f;
        }

        private static float SoftStep(float value)
        {
            return 1.0f / (1.0f + Mathf.Exp(-value));
        }

        private static float TanH(float value)
        {
            return 2.0f / (1.0f + Mathf.Exp(-value * 2.0f)) - 1.0f;
        }

        private static float ArcTan(float value)
        {
            return Mathf.Atan(value);
        }

        private static float SoftSign(float value)
        {
            return value / 1 + Mathf.Abs(value);
        }
    }

    public enum ActivationFunctionType
    {
        Identity,
        BinaryStep,
        SoftStep,
        TanH,
        ArcTan,
        SoftSign,
    }
}