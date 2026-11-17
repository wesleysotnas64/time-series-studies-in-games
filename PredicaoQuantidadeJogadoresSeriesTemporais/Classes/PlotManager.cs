using Raylib_cs;

namespace PredicaoQuantidadeJogadoresSeriesTemporais.Classes;

public class PlotManager
{
    
    
    public void PlotDataTopWindow(List<int> data, ModelML? model)
    {
        if (data == null || data.Count == 0)
            return;

        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        int left = 70;
        int right = screenWidth - 30;
        int top = 40;
        int bottom = screenHeight / 2 - 60;

        int graphWidth = right - left;
        int graphHeight = bottom - top;

        // -----------------------------------------
        // PREVISÕES DA REDE NEURAL
        // -----------------------------------------

        List<(int Month, double Value)> predictions = new();

        if (model != null && data.Count > model.InputCount)
        {
            // Percorre os dados criando janelas deslizantes.
            for (int i = 0; i + model.InputCount < data.Count; i++)
            {
                // Obtém os seis meses anteriores.
                double[] inputs = data
                    .Skip(i)
                    .Take(model.InputCount)
                    .Select(value => (double)value)
                    .ToArray();

                // A previsão utiliza os dados originais.
                // Predict realiza a normalização internamente.
                double prediction = model.Predict(inputs);

                // A previsão corresponde ao mês seguinte
                // ao último mês da janela de entrada.
                int predictedMonth = i + model.InputCount + 1;

                predictions.Add((predictedMonth, prediction));
            }
        }

        // -----------------------------------------
        // ESCALA VERTICAL
        // -----------------------------------------

        double minValue = data.Min();
        double maxValue = data.Max();

        if (predictions.Count > 0)
        {
            minValue = Math.Min(
                minValue,
                predictions.Min(p => p.Value)
            );

            maxValue = Math.Max(
                maxValue,
                predictions.Max(p => p.Value)
            );
        }

        if (maxValue == minValue)
        {
            maxValue += 1;
            minValue -= 1;
        }

        // Adiciona uma margem para as linhas não
        // ficarem encostadas nas bordas do gráfico.
        double margin = (maxValue - minValue) * 0.05;

        minValue -= margin;
        maxValue += margin;

        // Converte um valor para a coordenada Y.
        float GetY(double value)
        {
            return bottom -
                (float)((value - minValue) / (maxValue - minValue))
                * graphHeight;
        }

        // Converte o índice do mês para a coordenada X.
        float GetX(int month)
        {
            if (data.Count == 1)
                return left;

            return left +
                (float)(month - 1) / (data.Count - 1) * graphWidth;
        }

        // -----------------------------------------
        // LINHAS HORIZONTAIS E VALORES DO EIXO Y
        // -----------------------------------------

        int divisions = 5;

        for (int i = 0; i <= divisions; i++)
        {
            float percentage = (float)i / divisions;

            int y = bottom - (int)(percentage * graphHeight);

            double value = minValue +
                percentage * (maxValue - minValue);

            Raylib.DrawLine(
                left,
                y,
                right,
                y,
                Color.DarkGray
            );

            Raylib.DrawText(
                value.ToString("N0"),
                5,
                y - 8,
                15,
                Color.White
            );
        }

        // -----------------------------------------
        // EIXOS
        // -----------------------------------------

        Raylib.DrawLine(left, top, left, bottom, Color.White);
        Raylib.DrawLine(left, bottom, right, bottom, Color.White);

        // -----------------------------------------
        // EIXO X: MESES
        // -----------------------------------------

        float monthSpacing = data.Count > 1
            ? (float)graphWidth / (data.Count - 1)
            : graphWidth;

        // Mantém os números legíveis quando existem
        // muitos meses no gráfico.
        int labelInterval = Math.Max(
            1,
            (int)Math.Ceiling(30.0 / monthSpacing)
        );

        for (int i = 0; i < data.Count; i++)
        {
            float xPosition = GetX(i + 1);

            // Marca o mês no eixo.
            Raylib.DrawLine(
                (int)xPosition,
                bottom,
                (int)xPosition,
                bottom + 4,
                Color.White
            );

            // Número do mês.
            if (i % labelInterval == 0 || i == data.Count - 1)
            {
                string month = (i + 1).ToString();

                int textWidth = Raylib.MeasureText(month, 10);

                Raylib.DrawText(
                    month,
                    (int)xPosition - textWidth / 2,
                    bottom + 8,
                    10,
                    Color.White
                );
            }
        }

        // -----------------------------------------
        // LINHA DOS DADOS REAIS
        // -----------------------------------------

        for (int i = 0; i < data.Count; i++)
        {
            float x = GetX(i + 1);
            float y = GetY(data[i]);

            // Desenha a conexão entre os meses.
            if (i > 0)
            {
                float previousX = GetX(i);
                float previousY = GetY(data[i - 1]);

                Raylib.DrawLine(
                    (int)previousX,
                    (int)previousY,
                    (int)x,
                    (int)y,
                    Color.Green
                );
            }

            // Marca o valor real.
            Raylib.DrawCircle(
                (int)x,
                (int)y,
                3,
                Color.Green
            );
        }

        // -----------------------------------------
        // LINHA DAS PREVISÕES DA REDE NEURAL
        // -----------------------------------------

        for (int i = 0; i < predictions.Count; i++)
        {
            float x = GetX(predictions[i].Month);
            float y = GetY(predictions[i].Value);

            if (i > 0)
            {
                float previousX = GetX(predictions[i - 1].Month);
                float previousY = GetY(predictions[i - 1].Value);

                Raylib.DrawLine(
                    (int)previousX,
                    (int)previousY,
                    (int)x,
                    (int)y,
                    Color.Orange
                );
            }

            Raylib.DrawCircle(
                (int)x,
                (int)y,
                3,
                Color.Orange
            );
        }

        // -----------------------------------------
        // LEGENDA
        // -----------------------------------------

        int legendY = 12;

        Raylib.DrawLine(
            screenWidth - 300,
            legendY + 7,
            screenWidth - 280,
            legendY + 7,
            Color.Green
        );

        Raylib.DrawText(
            "Dados reais",
            screenWidth - 275,
            legendY,
            12,
            Color.White
        );

        Raylib.DrawLine(
            screenWidth - 170,
            legendY + 7,
            screenWidth - 150,
            legendY + 7,
            Color.Orange
        );

        Raylib.DrawText(
            "Rede neural",
            screenWidth - 145,
            legendY,
            12,
            Color.White
        );
    }



    // Painel inferior esquerdo: informações do modelo.
    public bool PlotModelInfoBottomLeft(ModelML model)
    {
        if (model == null)
            return false;

        int screenHeight = Raylib.GetScreenHeight();

        int panelX = 10;
        int panelY = screenHeight / 2 + 10;
        int panelWidth = Raylib.GetScreenWidth() / 2 - 15;
        int panelHeight = screenHeight - panelY - 10;

        // Borda externa.
        Raylib.DrawRectangleLines(
            panelX,
            panelY,
            panelWidth,
            panelHeight,
            Color.White
        );

        int leftColumnX = panelX + 10;
        int rightColumnX = panelX + panelWidth / 2 + 5;

        int top = panelY + 10;

        // Divisão vertical das duas colunas.
        Raylib.DrawLine(
            panelX + panelWidth / 2,
            panelY + 5,
            panelX + panelWidth / 2,
            panelY + panelHeight - 5,
            Color.DarkGray
        );

        // -----------------------------------------
        // COLUNA ESQUERDA: CONFIGURAÇÕES E ENTRADAS
        // -----------------------------------------

        int y = top;

        DrawText(
            "CONFIGURACAO DO MODELO",
            leftColumnX,
            y,
            11,
            Color.SkyBlue
        );

        y += 22;

        DrawText(
            $"Arquitetura: {model.InputCount}-{model.HiddenNeuronCount}-{model.OutputCount}",
            leftColumnX,
            y,
            10,
            Color.White
        );

        y += 17;

        DrawText(
            $"Min: {model.MinValue:F0}  Max: {model.MaxValue:F0}",
            leftColumnX,
            y,
            10,
            Color.White
        );

        y += 17;

        DrawText(
            $"Treino: {model.TrainingData.Count} amostras",
            leftColumnX,
            y,
            10,
            Color.White
        );

        y += 17;

        DrawText(
            $"Validacao: {model.ValidationData.Count}",
            leftColumnX,
            y,
            10,
            Color.White
        );

        y += 17;

        DrawText(
            $"Teste: {model.TestData.Count}",
            leftColumnX,
            y,
            10,
            Color.White
        );

        y += 25;

        DrawText(
            "CAMADA DE ENTRADA",
            leftColumnX,
            y,
            11,
            Color.SkyBlue
        );

        y += 18;

        // Exibe as seis entradas em duas linhas.
        for (int i = 0; i < model.InputLayer.Inputs.Length; i += 3)
        {
            string values = string.Join(
                "  ",
                model.InputLayer.Inputs
                    .Skip(i)
                    .Take(3)
                    .Select(value => value.ToString("F3"))
            );

            DrawText(
                values,
                leftColumnX,
                y,
                9,
                Color.White
            );

            y += 15;
        }

        y += 15;

        DrawText(
            "TREINAMENTO",
            leftColumnX,
            y,
            11,
            Color.SkyBlue
        );

        y += 20;

        DrawText(
            $"Epoca: {model.CurrentEpoch}/{model.TotalEpochs}",
            leftColumnX,
            y,
            10,
            Color.White
        );

        y += 17;

        DrawText(
            $"Amostra: {model.CurrentTrainingSample}/{model.TotalTrainingSamples}",
            leftColumnX,
            y,
            9,
            Color.White
        );

        y += 17;

        string state = model.IsTrainingCompleted
            ? "Finalizado"
            : model.IsTraining
                ? "Em andamento"
                : "Aguardando";

        DrawText(
            $"Estado: {state}",
            leftColumnX,
            y,
            10,
            Color.White
        );

        // -----------------------------------------
        // COLUNA DIREITA: NEURONIOS E SEUS PESOS
        // -----------------------------------------

        y = top;

        DrawText(
            "NEURONIOS - CAMADA OCULTA",
            rightColumnX,
            y,
            10,
            Color.SkyBlue
        );

        y += 20;

        for (int i = 0; i < model.HiddenLayer.Neurons.Length; i++)
        {
            Neuron neuron = model.HiddenLayer.Neurons[i];

            DrawText(
                $"H{i + 1}  Z:{neuron.Z:F3}  O:{neuron.Output:F3}",
                rightColumnX,
                y,
                9,
                Color.White
            );

            y += 14;

            DrawText(
                $"B:{neuron.Bias:F3}",
                rightColumnX,
                y,
                9,
                Color.LightGray
            );

            y += 14;

            // Exibe os seis pesos em duas linhas.
            for (int j = 0; j < neuron.Weights.Length; j += 3)
            {
                string weights = string.Join(
                    " ",
                    neuron.Weights
                        .Skip(j)
                        .Take(3)
                        .Select(weight => weight.ToString("F2"))
                );

                DrawText(
                    weights,
                    rightColumnX,
                    y,
                    8,
                    Color.LightGray
                );

                y += 12;
            }

            y += 5;
        }

        // Neurônio da camada de saída.
        DrawText(
            "CAMADA DE SAIDA",
            rightColumnX,
            y,
            10,
            Color.SkyBlue
        );

        y += 16;

        for (int i = 0; i < model.OutputLayer.Neurons.Length; i++)
        {
            Neuron neuron = model.OutputLayer.Neurons[i];

            DrawText(
                $"O{i + 1} Z:{neuron.Z:F3} O:{neuron.Output:F3}",
                rightColumnX,
                y,
                9,
                Color.White
            );

            y += 14;

            DrawText(
                $"B:{neuron.Bias:F3}",
                rightColumnX,
                y,
                9,
                Color.LightGray
            );

            y += 14;

            string weights = string.Join(
                " ",
                neuron.Weights.Select(weight => weight.ToString("F2"))
            );

            DrawText(
                weights,
                rightColumnX,
                y,
                8,
                Color.LightGray
            );
        }

        // Botão de início.
        int buttonWidth = 180;
        int buttonHeight = 30;

        int buttonX = panelX + 10;
        int buttonY = panelY + panelHeight - buttonHeight - 10;

        bool mouseOver =
            Raylib.GetMouseX() >= buttonX &&
            Raylib.GetMouseX() <= buttonX + buttonWidth &&
            Raylib.GetMouseY() >= buttonY &&
            Raylib.GetMouseY() <= buttonY + buttonHeight;

        Color buttonColor = model.IsTraining || model.IsTrainingCompleted
            ? Color.DarkGray
            : mouseOver
                ? Color.Green
                : Color.DarkGreen;

        Raylib.DrawRectangle(
            buttonX,
            buttonY,
            buttonWidth,
            buttonHeight,
            buttonColor
        );

        Raylib.DrawRectangleLines(
            buttonX,
            buttonY,
            buttonWidth,
            buttonHeight,
            Color.White
        );

        string buttonText = "Iniciar Treinamento";

        int textWidth = Raylib.MeasureText(buttonText, 12);

        Raylib.DrawText(
            buttonText,
            buttonX + (buttonWidth - textWidth) / 2,
            buttonY + 9,
            12,
            Color.White
        );

        // Só permite iniciar uma vez.
        return mouseOver &&
               Raylib.IsMouseButtonPressed(MouseButton.Left) &&
               !model.IsTraining &&
               !model.IsTrainingCompleted;
    }

    // Painel inferior direito: época e gráfico de Loss.
    public void PlotTrainingBottomRight(ModelML model)
    {
        if (model == null)
            return;

        int screenWidth = Raylib.GetScreenWidth();
        int screenHeight = Raylib.GetScreenHeight();

        int panelX = screenWidth / 2 + 5;
        int panelY = screenHeight / 2 + 10;
        int panelWidth = screenWidth - panelX - 10;
        int panelHeight = screenHeight - panelY - 10;

        // Borda externa.
        Raylib.DrawRectangleLines(
            panelX,
            panelY,
            panelWidth,
            panelHeight,
            Color.White
        );

        int left = panelX + 45;
        int right = panelX + panelWidth - 15;
        int top = panelY + 65;
        int bottom = panelY + panelHeight - 45;

        int graphWidth = right - left;
        int graphHeight = bottom - top;

        // Informações de época e amostra.
        DrawText(
            $"EPOCA: {model.CurrentEpoch}/{model.TotalEpochs}",
            panelX + 15,
            panelY + 12,
            15,
            Color.White
        );

        DrawText(
            $"Amostra: {model.CurrentTrainingSample}/{model.TotalTrainingSamples}",
            panelX + 15,
            panelY + 35,
            11,
            Color.LightGray
        );

        // Obtém cópias dos históricos.
        var lossHistory = model.GetLossHistory();

        List<double> trainingLoss = lossHistory.Training;
        List<double> validationLoss = lossHistory.Validation;

        // Determina a escala do gráfico.
        double maxValue = 1;

        if (trainingLoss.Count > 0 || validationLoss.Count > 0)
        {
            maxValue = trainingLoss
                .Concat(validationLoss)
                .Max();

            if (maxValue <= 0)
                maxValue = 1;
        }

        // Eixos.
        Raylib.DrawLine(left, top, left, bottom, Color.White);
        Raylib.DrawLine(left, bottom, right, bottom, Color.White);

        // Linhas horizontais.
        int divisions = 5;

        for (int i = 0; i <= divisions; i++)
        {
            float percentage = (float)i / divisions;

            int y = bottom - (int)(percentage * graphHeight);

            double value = percentage * maxValue;

            Raylib.DrawLine(
                left,
                y,
                right,
                y,
                Color.DarkGray
            );

            DrawText(
                value.ToString("F3"),
                panelX + 2,
                y - 7,
                8,
                Color.White
            );
        }

        // Gráfico de Training Loss.
        if (trainingLoss.Count >= 2)
        {
            DrawLossLine(
                trainingLoss,
                maxValue,
                left,
                bottom,
                graphWidth,
                graphHeight,
                Color.Blue
            );
        }

        // Gráfico de Validation Loss.
        if (validationLoss.Count >= 2)
        {
            DrawLossLine(
                validationLoss,
                maxValue,
                left,
                bottom,
                graphWidth,
                graphHeight,
                Color.Orange
            );
        }

        // Legenda.
        int legendY = panelY + panelHeight - 25;

        Raylib.DrawCircle(
            panelX + 25,
            legendY + 5,
            4,
            Color.Blue
        );

        DrawText(
            "Training Loss",
            panelX + 35,
            legendY,
            10,
            Color.White
        );

        Raylib.DrawCircle(
            panelX + 165,
            legendY + 5,
            4,
            Color.Orange
        );

        DrawText(
            "Validation Loss",
            panelX + 175,
            legendY,
            10,
            Color.White
        );
    }

    private void DrawLossLine(
        List<double> losses,
        double maxValue,
        int left,
        int bottom,
        int graphWidth,
        int graphHeight,
        Color color)
    {
        if (losses.Count < 2)
            return;

        for (int i = 0; i < losses.Count; i++)
        {
            float xPosition = left +
                (float)i / (losses.Count - 1) * graphWidth;

            float yPosition = bottom -
                (float)(losses[i] / maxValue) * graphHeight;

            if (i > 0)
            {
                float previousX = left +
                    (float)(i - 1) / (losses.Count - 1) * graphWidth;

                float previousY = bottom -
                    (float)(losses[i - 1] / maxValue) * graphHeight;

                Raylib.DrawLine(
                    (int)previousX,
                    (int)previousY,
                    (int)xPosition,
                    (int)yPosition,
                    color
                );
            }

            Raylib.DrawCircle(
                (int)xPosition,
                (int)yPosition,
                3,
                color
            );
        }
    }

    private void DrawText(
        string text,
        int x,
        int y,
        int fontSize,
        Color color)
    {
        Raylib.DrawText(
            text,
            x,
            y,
            fontSize,
            color
        );
    }
}
