﻿using System;
using SharpLearning.Neural.Initializations;
using SharpLearning.Neural.Providers.DotNetOp;

namespace SharpLearning.Neural.LayersNew
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DenseLayer : ILayerNew
    {
        readonly int m_units;

        Variable Weights;
        Variable Bias;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="units"></param>
        public DenseLayer(int units)
        {
            if (units < 1) { throw new ArgumentException("HiddenLayer must have at least 1 hidden unit"); }
            m_units = units;
        }

        /// <summary>
        /// 
        /// </summary>
        public Variable Output { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Variable Input { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executor"></param>
        public void Forward(Executor executor)
        {
            Dense.Forward(Input, Weights, Bias,
                Output, executor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="executor"></param>
        public void Backward(Executor executor)
        {
            Dense.Backward(Input, Weights, Bias,
                Output, executor);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputVariable"></param>
        /// <param name="excecutor"></param>
        /// <param name="random"></param>
        /// <param name="initializtion"></param>
        public void Initialize(Variable inputVariable, Executor excecutor, Random random,
            Initialization initializtion = Initialization.GlorotUniform)
        {
            Input = inputVariable;

            var batchSize = inputVariable.Dimensions[0];
            var fanIn = inputVariable.DimensionOffSets[0]; // product of all dimensions except batch size.
            var fanOut = m_units;

            var fans = new FanInFanOut(fanIn, fanOut);
            var distribution = WeightInitialization.GetWeightDistribution(initializtion, fans, random);
                        
            Weights = Variable.CreateTrainable(fans.FanIn, fans.FanOut);
            excecutor.AssignTensor(Weights, () => (float)distribution.Sample());

            Bias = Variable.CreateTrainable(fans.FanOut);

            Output = Variable.Create(batchSize, fans.FanOut);
        }
    }
}