using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FES_data_generator.Utils
{
    //public enum State { True, False }
    internal class MarkovChain
    {
        private readonly double[,] _transitionProbabilities;
        private Random r = new Random();

        public MarkovChain(double trueToTrueProbability, double falseToTrueProbability)
        {
            _transitionProbabilities = new double[2, 2]
            {
            { trueToTrueProbability, 1 - trueToTrueProbability },
            { falseToTrueProbability, 1 - falseToTrueProbability }
            };
        }

        public bool NextState(bool currentState)
        {
            double randomNumber = r.NextDouble();

            if (currentState == true)
            {
                if (randomNumber < _transitionProbabilities[0, 0])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (randomNumber < _transitionProbabilities[1, 0])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool[] GenerateStates(bool init, int length)
        {
            var states = new bool[length];
            states[0] = init;
            for (int i = 1; i < length; i++)
            {
                states[i] = NextState(states[i - 1]);
            }
            return states;
        }

    }
}
