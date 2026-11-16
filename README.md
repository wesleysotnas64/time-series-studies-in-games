# 📈 Time Series Study In Games 🎮

Bem-vindo ao meu repositório de estudos sobre **Séries Temporais (Time Series)**! 🚀

Este espaço foi criado para documentar a minha jornada de aprendizado na análise e previsão de dados sequenciais. O foco principal dos estudos está voltado para a previsão de valores numéricos (*forecasting*) e classificação de estados de jogo (como vitória ou derrota de equipes), aplicando técnicas de ciência de dados e aprendizado de máquina.

---

## ⏳ O que são Séries Temporais?

Uma **série temporal** é uma sequência de dados coletados, registrados ou observados em intervalos regulares ao longo do tempo (por exemplo: por segundo, hora, dia ou semana). 

Diferente de dados comuns onde a ordem das linhas não importa, nas séries temporais **o tempo é uma variável fundamental**. O objetivo de estudá-las é identificar padrões históricos — como tendências de crescimento, sazonalidades (ciclos que se repetem) e comportamentos anômalos — para entender o passado e prever eventos futuros.

---

## 🕹️ Séries Temporais no Contexto de Jogos Digitais

Como profissional e entusiasta da área de desenvolvimento de jogos, criei este repositório com o objetivo de **trazer os conceitos, práticas e estudos de séries temporais para o universo dos games**. 

Embora já existam diversas pesquisas avançadas sobre o tema, estou iniciando agora minha imersão nessa área e quero compartilhar publicamente o que estou aprendendo, testando e construindo. Os jogos digitais são ambientes extremamente ricos em dados temporais, permitindo aplicações práticas como:

* 📊 **Previsão de audiência:** Estimar picos de jogadores simultâneos e retenção ao longo das semanas.
* 🏆 **Análise competitiva (Esports):** Prever probabilidades de vitória ou derrota com base na linha do tempo de uma partida (ouro, abates, objetivos conquistados minuto a minuto).
* ⚖️ **Balanceamento e Economia:** Acompanhar a inflação de moedas virtuais ou a taxa de escolha (*pick rate*) de personagens após atualizações (*patches*).

---

## 🗂️ Projetos do Repositório

Abaixo está a lista de projetos desenvolvidos neste repositório. Clique no nome do projeto para ver os detalhes ou acesse diretamente a pasta com o código-fonte.

| Projeto | Foco Principal | Status |
| :--- | :--- | :--- |
| [Predição de Quantidade de Jogadores](#predição-de-quantidade-de-jogadores-usando-séries-temporais) | Previsão Numérica (MLP / Redes Neurais) | 🚧 Em desenvolvimento |

---

## 🔍 Detalhes dos Projetos

### Predição de Quantidade de Jogadores Usando Séries Temporais

📂 **Acesse a pasta do projeto:** [`./PredicaoQuantidadeJogadoresSeriesTemporais`](./PredicaoQuantidadeJogadoresSeriesTemporais)

#### 📝 Descrição
Este projeto tem como objetivo estudar e implementar técnicas de aprendizado de máquina para a previsão da quantidade diária de jogadores de um jogo disponível na plataforma Steam, utilizando séries temporais e redes neurais artificiais.

O problema consiste em analisar o histórico de jogadores de um determinado jogo, considerando a quantidade de jogadores registrada diariamente, para identificar padrões temporais e estimar valores futuros. Os dados históricos serão obtidos a partir do **SteamCharts**, considerando o pico diário de jogadores ao longo de aproximadamente **365 dias**.

#### 🧠 Abordagem
A solução será desenvolvida utilizando uma rede neural artificial do tipo **Multilayer Perceptron (MLP)**, com aprendizado supervisionado por retropropagação (*Backpropagation*).

Será utilizada uma abordagem de janela deslizante (*Sliding Window*), na qual os **7 últimos valores conhecidos** da série temporal são fornecidos à rede neural para estimar a quantidade de jogadores do dia seguinte:

* **Camada de entrada:** 7 neurônios (correspondentes aos 7 dias anteriores).
* **Camada oculta:** 4 neurônios (função de ativação **ReLU**).
* **Camada de saída:** 1 neurônio (ativação **linear** para produzir a previsão numérica).
* **Treinamento:** Erro Quadrático Médio (**MSE**) como função de perda e retropropagação para atualização dos pesos e *biases*.

#### 📊 Divisão dos Dados
O histórico de 52 semanas será organizado cronologicamente e dividido em três conjuntos:
* **Treinamento (36 semanas):** Utilizadas para ajustar os parâmetros da rede.
* **Validação (8 semanas):** Utilizadas para avaliar o desempenho durante o desenvolvimento e orientar ajustes de hiperparâmetros.
* **Teste (8 semanas):** Utilizadas para avaliar a capacidade de generalização do modelo após a configuração final.

#### 🖥️ Visualização
A biblioteca **Raylib-Cs** será utilizada para desenvolver uma interface gráfica interativa que permita acompanhar o processo de treinamento em tempo real e analisar o comportamento do modelo, exibindo:
* Evolução dos valores reais da série temporal vs. valores previstos.
* Curva de evolução do erro (MSE) durante o treinamento.
* Desempenho do modelo nos conjuntos de validação e teste.

#### 🎯 Objetivo de Aprendizagem
Além de desenvolver um modelo funcional de previsão, o projeto busca aprofundar o entendimento prático sobre redes neurais artificiais construídas do zero aplicadas a séries temporais, abordando preparação dos dados, normalização, janelas deslizantes, regressão, funções de ativação, retropropagação, avaliação de modelos e visualização do processo de aprendizado.
