using PredicaoQuantidadeJogadoresSeriesTemporais.Classes;
using PredicaoQuantidadeJogadoresSeriesTemporais.Data;
using Raylib_cs;

Raylib.InitWindow(800, 600, "Prediction Quantity Players");

// Permite redimensionar a janela.
Raylib.SetWindowState(ConfigFlags.ResizableWindow);

// Maximiza sem entrar em tela cheia.
Raylib.MaximizeWindow();

Raylib.SetTargetFPS(120);

DataManager dataManager = new DataManager();
PlotManager plotManager = new PlotManager();

List<int> players = new List<int>();

// Inicia a extração sem bloquear a janela.
Task<List<int>> extractionTask = dataManager.DataExtraction(60, 730);

bool dataLoaded = false;
bool modelCreated = false;
bool trainingStarted = false;

string status = "Carregando dados da API...";

ModelML? model = null;

// Configurações do treinamento.
int epochs = 1000;
double learningRate = 0.001;

// Loop principal da janela.
while (!Raylib.WindowShouldClose())
{
    // Verifica se a API terminou de responder.
    if (!dataLoaded && extractionTask.IsCompleted)
    {
        try
        {
            players = extractionTask.GetAwaiter().GetResult();

            dataLoaded = true;
            status = "Dados carregados com sucesso!";
        }
        catch (Exception ex)
        {
            status = $"Erro ao carregar dados: {ex.Message}";
            dataLoaded = true;
        }
    }

    // Cria o modelo após carregar os dados.
    if (dataLoaded && !modelCreated && players.Count > 0)
    {
        try
        {
            model = new ModelML(players);

            modelCreated = true;
            status = "Modelo criado com sucesso!";
        }
        catch (Exception ex)
        {
            status = $"Erro ao criar modelo: {ex.Message}";
            modelCreated = true;
        }
    }

    // Executa uma amostra do treinamento por frame.
    if (model != null && model.IsTraining)
    {
        model.TrainNextStep();
    }

    // Atualiza o estado do treinamento.
    if (model != null && model.IsTrainingCompleted)
    {
        status = "Treinamento concluído!";
    }

    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);

    if (!dataLoaded || model == null)
    {
        // Exibe o estado da extração ou criação do modelo.
        Raylib.DrawText(
            status,
            30,
            30,
            20,
            Color.White
        );
    }
    else
    {
        // Gráfico dos dados históricos na parte superior.
        plotManager.PlotDataTopWindow(players, model);

        // Painel inferior esquerdo:
        // informações do modelo e botão de treinamento.
        bool startTraining = plotManager.PlotModelInfoBottomLeft(model);

        // Painel inferior direito:
        // informações das épocas e gráfico de Loss.
        plotManager.PlotTrainingBottomRight(model);

        // Inicia o treinamento somente após clicar no botão.
        if (startTraining && !trainingStarted)
        {
            trainingStarted = true;

            status = "Treinando modelo...";

            // Inicializa o treinamento.
            model.BeginTraining(epochs, learningRate);
        }

        // Status geral do programa.
        Raylib.DrawText(
            status,
            30,
            Raylib.GetScreenHeight() - 25,
            15,
            model.IsTrainingCompleted ? Color.Green : Color.White
        );
    }

    Raylib.EndDrawing();
}

Raylib.CloseWindow();
