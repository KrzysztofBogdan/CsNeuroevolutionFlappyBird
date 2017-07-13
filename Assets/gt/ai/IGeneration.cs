namespace gt.ai
{
    /// <summary>
    /// Create Generation using <see cref="NeuroevolutionGeneration.Create">NeuroevolutionGeneration.Create</see>
    /// Parematers are explained in <see cref="AiController"/>
    /// </summary>
    public interface IGeneration
    {
        /// <summary>
        /// Compute output for specified genome
        /// </summary>
        /// <param name="genomeNumber">0 based genome index</param>
        /// <param name="input">Inputs</param>
        /// <returns></returns>
        float[] Compute(int genomeNumber, float[] input);

        /// <summary>
        /// Genrates new generation based on prvided score.
        /// </summary>
        /// <param name="scores">Index of score in array defines genomeNumber</param>
        IGeneration NextGeneration(float[] scores);

        /// <summary>
        /// Save previous generation (not current). 
        /// Important: This methods sorts genomes by score provided by NextGeneration call
        /// Loading this data will put best genome on index (genomeNumber) 0.
        /// Load this data using <see cref="NeuroevolutionGeneration.Load">NeuroevolutionGeneration.Load</see>   
        /// </summary>
        /// <returns></returns>
        byte[] Save();
    }
}