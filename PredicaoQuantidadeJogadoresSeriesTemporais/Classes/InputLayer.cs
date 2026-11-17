
namespace PredicaoQuantidadeJogadoresSeriesTemporais.Classes;

public class InputLayer
{
    // Quantidade de entradas da camada.
    public int InputCount { get; private set; }

    // Valores recebidos pela camada de entrada.
    public double[] Inputs { get; private set; }

    // Construtor.
    public InputLayer(int inputCount)
    {
        InputCount = inputCount;
        Inputs = new double[inputCount];
    }

    // Recebe os valores da série temporal.
    public double[] Forward(double[] inputs)
    {
        if (inputs.Length != InputCount)
        {
            throw new ArgumentException(
                $"A camada de entrada espera {InputCount} valores."
            );
        }

        Inputs = inputs;

        return Inputs;
    }
}