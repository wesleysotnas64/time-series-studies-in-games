namespace PredicaoQuantidadeJogadoresSeriesTemporais.Classes;

public class OutputLayer
{
    // Neurônios da camada de saída.
    public Neuron[] Neurons { get; private set; }

    // Quantidade de neurônios da camada.
    public int NeuronCount { get; private set; }

    // Quantidade de entradas recebidas por cada neurônio.
    public int InputCount { get; private set; }

    // Saídas produzidas pelos neurônios.
    public double[] Outputs { get; private set; }

    // Construtor.
    public OutputLayer(int neuronCount, int inputCount)
    {
        NeuronCount = neuronCount;
        InputCount = inputCount;

        Neurons = new Neuron[neuronCount];
        Outputs = new double[neuronCount];

        for (int i = 0; i < neuronCount; i++)
        {
            Neurons[i] = new Neuron(inputCount);
        }
    }

    // Recebe as entradas e calcula a saída de cada neurônio.
    public double[] Forward(double[] inputs)
    {
        if (inputs.Length != InputCount)
        {
            throw new ArgumentException(
                $"A camada de saída espera {InputCount} entradas."
            );
        }

        for (int i = 0; i < NeuronCount; i++)
        {
            double z = Neurons[i].Calculate(inputs);

            // Utiliza função de ativação linear.
            Outputs[i] = Neurons[i].Activate(z, false);
        }

        return Outputs;
    }
}