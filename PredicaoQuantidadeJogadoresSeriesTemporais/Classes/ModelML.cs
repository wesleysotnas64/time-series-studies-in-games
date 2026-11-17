
using System;
using System.Collections.Generic;
using System.Linq;

namespace PredicaoQuantidadeJogadoresSeriesTemporais.Classes;

public class ModelML
{
    // Arquitetura da rede neural.
    public InputLayer InputLayer { get; private set; }
    public HiddenLayer HiddenLayer { get; private set; }
    public OutputLayer OutputLayer { get; private set; }

    // Configurações da arquitetura.
    public int InputCount { get; private set; } = 6;
    public int HiddenNeuronCount { get; private set; } = 4;
    public int OutputCount { get; private set; } = 1;

    // Conjunto completo de dados recebidos.
    public List<int> CompleteData { get; private set; }

    // Conjuntos de treinamento, validação e teste.
    public List<(double[] Inputs, double Target)> TrainingData { get; private set; }
    public List<(double[] Inputs, double Target)> ValidationData { get; private set; }
    public List<(double[] Inputs, double Target)> TestData { get; private set; }

    // Histórico de Loss.
    public List<double> TrainingLoss { get; private set; }
    public List<double> ValidationLoss { get; private set; }
    public List<double> TestLoss { get; private set; }

    // Limites utilizados na normalização.
    public double MinValue { get; private set; }
    public double MaxValue { get; private set; }

    // Controle do treinamento.
    public int CurrentEpoch { get; private set; }
    public int TotalEpochs { get; private set; }

    public int CurrentTrainingSample { get; private set; }

    public int TotalTrainingSamples => TrainingData.Count;

    public bool IsTraining { get; private set; }
    public bool IsTrainingCompleted { get; private set; }

    private double trainingLearningRate;

    // Construtor.
    public ModelML(List<int> data)
    {
        if (data == null || data.Count < InputCount + 1)
        {
            throw new ArgumentException(
                "São necessários pelo menos 7 meses de dados."
            );
        }

        CompleteData = data;

        // Inicializa as camadas da rede neural.
        InputLayer = new InputLayer(InputCount);
        HiddenLayer = new HiddenLayer(HiddenNeuronCount, InputCount);
        OutputLayer = new OutputLayer(OutputCount, HiddenNeuronCount);

        // Inicializa os conjuntos de dados.
        TrainingData = new List<(double[] Inputs, double Target)>();
        ValidationData = new List<(double[] Inputs, double Target)>();
        TestData = new List<(double[] Inputs, double Target)>();

        // Inicializa os históricos de Loss.
        TrainingLoss = new List<double>();
        ValidationLoss = new List<double>();
        TestLoss = new List<double>();

        // Prepara e divide os dados.
        PrepareData();

        // Normaliza os conjuntos.
        NormalizeData();
    }

    // Cria as amostras e divide os conjuntos de dados.
    private void PrepareData()
    {
        List<(double[] Inputs, double Target)> samples =
            new List<(double[] Inputs, double Target)>();

        // Criação das amostras utilizando janela deslizante.
        for (int i = 0; i + InputCount < CompleteData.Count; i++)
        {
            double[] inputs = CompleteData
                .Skip(i)
                .Take(InputCount)
                .Select(value => (double)value)
                .ToArray();

            double target = CompleteData[i + InputCount];

            samples.Add((inputs, target));
        }

        // Calcula a quantidade de amostras para validação e teste.
        int validationCount = (int)Math.Round(
            samples.Count * 0.15,
            MidpointRounding.AwayFromZero
        );

        int testCount = (int)Math.Round(
            samples.Count * 0.15,
            MidpointRounding.AwayFromZero
        );

        int trainingCount = samples.Count - validationCount - testCount;

        if (trainingCount <= 0 || validationCount <= 0 || testCount <= 0)
        {
            throw new ArgumentException(
                "Não existem amostras suficientes para dividir os conjuntos."
            );
        }

        // Divide os conjuntos sem embaralhar os dados.
        TrainingData = samples.GetRange(0, trainingCount);

        ValidationData = samples.GetRange(
            trainingCount,
            validationCount
        );

        TestData = samples.GetRange(
            trainingCount + validationCount,
            testCount
        );
    }

    // Normaliza os dados utilizando os limites do treinamento.
    private void NormalizeData()
    {
        // Obtém todos os valores presentes nas amostras de treinamento.
        List<double> trainingValues = TrainingData
            .SelectMany(sample => sample.Inputs.Append(sample.Target))
            .ToList();

        // Calcula os limites exclusivamente com os dados de treinamento.
        MinValue = trainingValues.Min();
        MaxValue = trainingValues.Max();

        // Normaliza os três conjuntos.
        TrainingData = NormalizeSamples(TrainingData);
        ValidationData = NormalizeSamples(ValidationData);
        TestData = NormalizeSamples(TestData);
    }

    // Normaliza uma lista de amostras.
    private List<(double[] Inputs, double Target)> NormalizeSamples(
        List<(double[] Inputs, double Target)> samples)
    {
        List<(double[] Inputs, double Target)> normalizedSamples =
            new List<(double[] Inputs, double Target)>();

        foreach (var sample in samples)
        {
            double[] normalizedInputs = sample.Inputs
                .Select(NormalizeValue)
                .ToArray();

            double normalizedTarget = NormalizeValue(sample.Target);

            normalizedSamples.Add((
                normalizedInputs,
                normalizedTarget
            ));
        }

        return normalizedSamples;
    }

    // Transforma um valor original para o intervalo normalizado.
    private double NormalizeValue(double value)
    {
        if (MaxValue == MinValue)
            return 0;

        return (value - MinValue) / (MaxValue - MinValue);
    }

    // Recupera o valor original a partir do valor normalizado.
    private double DenormalizeValue(double value)
    {
        return value * (MaxValue - MinValue) + MinValue;
    }

    // Executa a propagação para frente utilizando dados normalizados.
    private double PredictNormalized(double[] inputs)
    {
        double[] inputOutputs = InputLayer.Forward(inputs);

        double[] hiddenOutputs = HiddenLayer.Forward(inputOutputs);

        double[] output = OutputLayer.Forward(hiddenOutputs);

        return output[0];
    }

    // Recebe valores originais e retorna a previsão na escala original.
    public double Predict(double[] inputs)
    {
        if (inputs == null || inputs.Length != InputCount)
        {
            throw new ArgumentException(
                $"A previsão espera {InputCount} valores."
            );
        }

        // Normaliza as entradas recebidas.
        double[] normalizedInputs = inputs
            .Select(NormalizeValue)
            .ToArray();

        // Realiza a previsão com valores normalizados.
        double normalizedPrediction = PredictNormalized(normalizedInputs);

        // Retorna a previsão para a escala original.
        return DenormalizeValue(normalizedPrediction);
    }

    // Calcula o erro quadrático médio.
    public double CalculateLoss(
        List<(double[] Inputs, double Target)> data)
    {
        if (data == null || data.Count == 0)
        {
            throw new ArgumentException(
                "O conjunto de dados não pode estar vazio."
            );
        }

        double totalLoss = 0;

        foreach (var sample in data)
        {
            // A amostra já está normalizada.
            double prediction = PredictNormalized(sample.Inputs);

            double error = sample.Target - prediction;

            totalLoss += Math.Pow(error, 2);
        }

        return totalLoss / data.Count;
    }

    // Executa a retropropagação e atualiza os parâmetros
    // utilizando uma única amostra de treinamento.
    public void TrainSample(
        double[] inputs,
        double target,
        double learningRate)
    {
        if (inputs == null || inputs.Length != InputCount)
        {
            throw new ArgumentException(
                $"A amostra deve possuir {InputCount} entradas."
            );
        }

        if (learningRate <= 0)
        {
            throw new ArgumentException(
                "A taxa de aprendizado deve ser maior que zero."
            );
        }

        // 1. Forward Pass.
        double[] inputOutputs = InputLayer.Forward(inputs);

        double[] hiddenOutputs = HiddenLayer.Forward(inputOutputs);

        double[] output = OutputLayer.Forward(hiddenOutputs);

        double prediction = output[0];

        // 2. Calcula o gradiente da camada de saída.
        Neuron outputNeuron = OutputLayer.Neurons[0];

        double outputDelta = 2 * (prediction - target);

        double[] outputWeightGradients =
            new double[outputNeuron.Weights.Length];

        for (int i = 0; i < outputNeuron.Weights.Length; i++)
        {
            outputWeightGradients[i] =
                outputDelta * hiddenOutputs[i];
        }

        double outputBiasGradient = outputDelta;

        // 3. Calcula os gradientes da camada oculta.
        double[] hiddenDeltas = new double[HiddenNeuronCount];

        double[][] hiddenWeightGradients =
            new double[HiddenNeuronCount][];

        double[] hiddenBiasGradients =
            new double[HiddenNeuronCount];

        for (int i = 0; i < HiddenNeuronCount; i++)
        {
            Neuron hiddenNeuron = HiddenLayer.Neurons[i];

            // Propaga o erro utilizando o peso da conexão
            // entre o neurônio oculto e o neurônio de saída.
            double propagatedError =
                outputDelta * outputNeuron.Weights[i];

            // Derivada da função ReLU.
            double reluDerivative =
                hiddenNeuron.Z > 0 ? 1 : 0;

            hiddenDeltas[i] =
                propagatedError * reluDerivative;

            // Calcula os gradientes dos pesos.
            hiddenWeightGradients[i] =
                new double[hiddenNeuron.Weights.Length];

            for (int j = 0; j < hiddenNeuron.Weights.Length; j++)
            {
                hiddenWeightGradients[i][j] =
                    hiddenDeltas[i] * inputOutputs[j];
            }

            // Gradiente do bias.
            hiddenBiasGradients[i] = hiddenDeltas[i];
        }

        // 4. Atualiza os parâmetros da camada de saída.
        outputNeuron.UpdateParameters(
            outputWeightGradients,
            outputBiasGradient,
            learningRate
        );

        // 5. Atualiza os parâmetros da camada oculta.
        for (int i = 0; i < HiddenNeuronCount; i++)
        {
            HiddenLayer.Neurons[i].UpdateParameters(
                hiddenWeightGradients[i],
                hiddenBiasGradients[i],
                learningRate
            );
        }
    }

    // Prepara o modelo para iniciar o treinamento.
    public void BeginTraining(int epochs, double learningRate)
    {
        if (epochs <= 0)
        {
            throw new ArgumentException(
                "A quantidade de épocas deve ser maior que zero."
            );
        }

        if (learningRate <= 0)
        {
            throw new ArgumentException(
                "A taxa de aprendizado deve ser maior que zero."
            );
        }

        if (TrainingData.Count == 0 ||
            ValidationData.Count == 0 ||
            TestData.Count == 0)
        {
            throw new InvalidOperationException(
                "Os conjuntos de treinamento, validação e teste não podem estar vazios."
            );
        }

        if (IsTraining)
        {
            throw new InvalidOperationException(
                "O treinamento já está em andamento."
            );
        }

        if (IsTrainingCompleted)
        {
            throw new InvalidOperationException(
                "O treinamento já foi concluído."
            );
        }

        // Limpa os históricos.
        TrainingLoss.Clear();
        ValidationLoss.Clear();
        TestLoss.Clear();

        // Configura as épocas.
        TotalEpochs = epochs;
        CurrentEpoch = 1;

        // Configura as amostras.
        CurrentTrainingSample = 0;

        // Armazena a taxa de aprendizado.
        trainingLearningRate = learningRate;

        // Atualiza o estado do treinamento.
        IsTraining = true;
        IsTrainingCompleted = false;
    }

    // Executa somente uma amostra de treinamento.
    // Deve ser chamado uma vez por frame.
    public void TrainNextStep()
    {
        if (!IsTraining || IsTrainingCompleted)
            return;

        // Obtém a amostra atual.
        var sample = TrainingData[CurrentTrainingSample];

        // Executa Forward Pass, Backpropagation
        // e atualização dos parâmetros.
        TrainSample(
            sample.Inputs,
            sample.Target,
            trainingLearningRate
        );

        // Avança para a próxima amostra.
        CurrentTrainingSample++;

        // Verifica se todas as amostras da época foram processadas.
        if (CurrentTrainingSample >= TotalTrainingSamples)
        {
            // Calcula o erro após atualizar os parâmetros.
            // Esses cálculos não realizam retropropagação.
            double trainingLoss = CalculateLoss(TrainingData);
            double validationLoss = CalculateLoss(ValidationData);

            // Armazena os resultados da época.
            TrainingLoss.Add(trainingLoss);
            ValidationLoss.Add(validationLoss);

            // Restaura as ativações para a última amostra treinada.
            var lastSample = TrainingData[TotalTrainingSamples - 1];

            double[] inputOutputs = InputLayer.Forward(lastSample.Inputs);

            double[] hiddenOutputs = HiddenLayer.Forward(inputOutputs);

            OutputLayer.Forward(hiddenOutputs);

            // Prepara a próxima época.
            CurrentTrainingSample = 0;

            if (CurrentEpoch >= TotalEpochs)
            {
                // Avalia o conjunto de teste somente ao final.
                double testLoss = CalculateLoss(TestData);

                TestLoss.Add(testLoss);

                // Restaura as ativações da última amostra de treinamento.
                inputOutputs = InputLayer.Forward(lastSample.Inputs);

                hiddenOutputs = HiddenLayer.Forward(inputOutputs);

                OutputLayer.Forward(hiddenOutputs);

                // Finaliza o treinamento.
                IsTraining = false;
                IsTrainingCompleted = true;
            }
            else
            {
                // Avança para a próxima época.
                CurrentEpoch++;
            }
        }
    }

    // Mantém uma versão síncrona do treinamento.
    // Útil caso seja necessário executar todas as épocas de uma vez.
    public void Train(int epochs, double learningRate)
    {
        BeginTraining(epochs, learningRate);

        while (IsTraining)
        {
            TrainNextStep();
        }
    }

    // Retorna cópias dos históricos de Loss.
    public (List<double> Training, List<double> Validation) GetLossHistory()
    {
        return (
            new List<double>(TrainingLoss),
            new List<double>(ValidationLoss)
        );
    }
}
