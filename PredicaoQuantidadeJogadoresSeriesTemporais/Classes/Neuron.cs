
namespace PredicaoQuantidadeJogadoresSeriesTemporais.Classes;

public class Neuron
{
    // Pesos associados às entradas do neurônio.
    public double[] Weights { get; private set; }

    // Bias do neurônio.
    public double Bias { get; private set; }

    // Valor recebido pelo neurônio após a soma ponderada.
    public double Z { get; private set; }

    // Resultado da função de ativação.
    public double Output { get; private set; }

    // Construtor.
    public Neuron(int inputCount)
    {
        Weights = new double[inputCount];

        Random random = new Random();

        for (int i = 0; i < inputCount; i++)
        {
            Weights[i] = random.NextDouble() * 2 - 1;
        }

        Bias = random.NextDouble() * 2 - 1;
    }

    // Calcula a soma ponderada das entradas e adiciona o bias.
    public double Calculate(double[] inputs)
    {
        Z = Bias;

        for (int i = 0; i < inputs.Length; i++)
        {
            Z += inputs[i] * Weights[i];
        }

        return Z;
    }

    // Função de ativação ReLU.
    public double ReLU(double z)
    {
        return Math.Max(0, z);
    }

    // Função de ativação linear.
    public double Linear(double z)
    {
        return z;
    }

    // Armazena a saída do neurônio utilizando uma função de ativação.
    public double Activate(double z, bool useReLU)
    {
        Output = useReLU ? ReLU(z) : Linear(z);

        return Output;
    }

    // Atualiza os pesos e o bias durante a retropropagação.
    public void UpdateParameters(double[] weightGradients, double biasGradient, double learningRate)
    {
        for (int i = 0; i < Weights.Length; i++)
        {
            Weights[i] -= learningRate * weightGradients[i];
        }

        Bias -= learningRate * biasGradient;
    }
}
