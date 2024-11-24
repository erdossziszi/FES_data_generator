namespace FES_data_generator.Utils;

public class MarkovChain
{
    private readonly double[,] _transitionProbabilities;
    private readonly Random _r = new();

    public MarkovChain(double trueToTrueProbability, double falseToTrueProbability)
    {
        _transitionProbabilities = new double[2, 2]
        {
            { trueToTrueProbability, 1 - trueToTrueProbability },
            { falseToTrueProbability, 1 - falseToTrueProbability }
        };
    }

    public int NextState(int currentState)
    {
        double randomNumber = _r.NextDouble();

        if (currentState == 1)
        {
            return randomNumber < _transitionProbabilities[0, 0] ? 1 : 0;
        }
        else
        {
            return randomNumber < _transitionProbabilities[1, 0] ? 1 : 0;
        }
    }

    public int[] GenerateStates(int initialState, int length)
    {
        var states = new int[length];
        states[0] = initialState;
        for (int i = 1; i < length; i++)
        {
            states[i] = NextState(states[i - 1]);
        }
        return states;
    }
}
