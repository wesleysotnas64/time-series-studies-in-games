# 📈 Time Series Study In Games 🎮

Bem-vindo ao meu repositório de estudos sobre **Séries Temporais (Time Series)**! 🚀

Este espaço foi criado para documentar minha jornada de aprendizado na análise e previsão de dados sequenciais. O foco principal dos estudos está voltado à previsão de valores numéricos (*forecasting*) e à classificação de estados de jogo, como vitória ou derrota de equipes, aplicando técnicas de ciência de dados e aprendizado de máquina.

---

## ⏳ O que são Séries Temporais?

Uma **série temporal** é uma sequência de dados coletados, registrados ou observados em intervalos regulares ao longo do tempo, como segundos, horas, dias, semanas ou meses.

Diferentemente de conjuntos de dados convencionais, nos quais a ordem das observações pode não ser relevante, nas séries temporais **o tempo é uma variável fundamental**. O objetivo de estudá-las é identificar padrões históricos, como tendências de crescimento, sazonalidades e comportamentos anômalos, para compreender o passado e realizar previsões futuras.

---

## 🕹️ Séries Temporais no Contexto de Jogos Digitais

Como profissional e entusiasta da área de desenvolvimento de jogos, criei este repositório com o objetivo de **explorar os conceitos e as aplicações de séries temporais no universo dos games**.

Os jogos digitais são ambientes ricos em dados temporais, permitindo diferentes aplicações práticas, como:

- 📊 **Previsão de audiência:** estimar a quantidade de jogadores ao longo do tempo, identificando tendências de crescimento e declínio da popularidade de um jogo.
- 🏆 **Análise competitiva (Esports):** prever probabilidades de vitória ou derrota com base na evolução dos acontecimentos de uma partida, como ouro, abates e objetivos conquistados.
- ⚖️ **Balanceamento e Economia:** acompanhar a evolução de moedas virtuais ou a taxa de escolha (*pick rate*) de personagens após atualizações (*patches*).

Embora existam diversas pesquisas avançadas sobre o tema, estou iniciando minha imersão nessa área e pretendo compartilhar publicamente o que estou aprendendo, testando e construindo.

---

## 🗂️ Projetos do Repositório

Abaixo está a lista de projetos desenvolvidos neste repositório. Clique no nome do projeto para consultar os detalhes ou acesse diretamente a pasta com o código-fonte.

| Projeto | Foco Principal | Status |
| :--- | :--- | :--- |
| [Predição de Quantidade de Jogadores](#predição-de-quantidade-de-jogadores-usando-séries-temporais) | Previsão Numérica (MLP / Redes Neurais) | 🚧 Em desenvolvimento |

---

## 🔍 Detalhes dos Projetos

### Predição de Quantidade de Jogadores Usando Séries Temporais

📂 **Acesse a pasta do projeto:** [`./PredicaoQuantidadeJogadoresSeriesTemporais`](./PredicaoQuantidadeJogadoresSeriesTemporais)

#### 📝 Descrição

Este projeto tem como objetivo estudar e implementar técnicas de aprendizado de máquina para a previsão da quantidade de jogadores de um jogo disponível na plataforma Steam, utilizando séries temporais e redes neurais artificiais.

O problema consiste em analisar o histórico mensal de jogadores de um determinado jogo para identificar padrões temporais e estimar a quantidade de jogadores em um mês futuro.

Os dados históricos serão obtidos a partir do **SteamCharts**, utilizando os registros mensais disponibilizados pela plataforma. A escolha da granularidade mensal permite trabalhar com um histórico mais amplo e consistente, evitando misturar registros de diferentes resoluções temporais, como dados horários, diários e mensais.

#### 🧠 Abordagem

A solução será desenvolvida utilizando uma rede neural artificial do tipo **Multilayer Perceptron (MLP)**, com aprendizado supervisionado por retropropagação (*Backpropagation*).

Será utilizada uma abordagem de janela deslizante (*Sliding Window*), na qual os **6 últimos valores mensais conhecidos** serão fornecidos à rede neural para estimar a quantidade de jogadores do mês seguinte.

A relação entre as entradas e a saída será:

- **Entradas:** valores correspondentes aos meses \(t-6\), \(t-5\), \(t-4\), \(t-3\), \(t-2\) e \(t-1\).
- **Saída esperada:** valor correspondente ao mês \(t\).

A arquitetura inicial da rede neural será composta por:

- **Camada de entrada:** 6 neurônios, correspondentes aos 6 meses anteriores.
- **Camada oculta:** 4 neurônios, utilizando a função de ativação **ReLU**.
- **Camada de saída:** 1 neurônio, utilizando ativação **linear** para produzir a previsão numérica.
- **Treinamento:** Erro Quadrático Médio (**MSE**) como função de perda e retropropagação para atualização dos pesos e *biases*.

#### 📊 Extração e Preparação dos Dados

A extração dos dados será realizada considerando intervalos mensais.

O parâmetro de extração representará a quantidade de meses históricos desejada, sem incluir o mês atual. Por exemplo:

- **12 meses:** retorna os últimos 12 meses completos.
- **24 meses:** retorna os últimos 24 meses completos.

Os valores serão organizados cronologicamente, garantindo que as observações sejam utilizadas na ordem correta durante a preparação das amostras.

A partir da série histórica, serão construídas amostras utilizando janelas deslizantes de 6 meses, nas quais cada conjunto de entradas será associado ao valor do mês imediatamente seguinte.

#### 📊 Divisão dos Dados

O histórico será dividido cronologicamente em três conjuntos:

- **Treinamento:** utilizado para ajustar os pesos e os *biases* da rede neural.
- **Validação:** utilizado para acompanhar o desempenho durante o desenvolvimento e orientar ajustes de hiperparâmetros.
- **Teste:** utilizado para avaliar a capacidade de generalização do modelo após a configuração final.

A quantidade de amostras de cada conjunto será definida de acordo com o tamanho do histórico mensal disponível.

A ordem temporal será preservada, evitando o embaralhamento das observações e o vazamento de informações futuras durante o treinamento e a avaliação.

#### 🖥️ Visualização

A biblioteca **Raylib-Cs** será utilizada para desenvolver uma interface gráfica que permita acompanhar o processo de treinamento e analisar o comportamento do modelo, exibindo:

- Evolução dos valores reais da série temporal em comparação com os valores previstos.
- Curva de evolução do erro (MSE) durante o treinamento.
- Desempenho do modelo nos conjuntos de validação e teste.

#### 🎯 Objetivo de Aprendizagem

Além de desenvolver um modelo funcional de previsão, o projeto busca aprofundar o entendimento prático sobre redes neurais artificiais construídas do zero e aplicadas a séries temporais.

Serão estudados conceitos como preparação de dados, normalização, janelas deslizantes, regressão, funções de ativação, retropropagação, avaliação de modelos e visualização do processo de aprendizado.