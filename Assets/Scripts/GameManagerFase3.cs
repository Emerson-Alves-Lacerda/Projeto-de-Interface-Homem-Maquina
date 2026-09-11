using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManagerFase3 : MonoBehaviour
{
    [Header("Tempo")]
    public float tempoRestante = 20f;

    [Header("UI Geral")]
    public Text txtTempo;
    public Text txtFeedback;
    public Text txtContagem;
    public float tempoFeedback = 0.8f;

    private Coroutine rotinaFeedback = null;
    private bool faseIniciada = false;

    [Header("Esquerda - Mini jogo visual")]
    public Text txtInstrucaoEsquerda;
    public Button btnEsq1;
    public Button btnEsq2;
    public Button btnEsq3;

    [Header("Cores de feedback")]
    public Color corErroAcessivel = new Color(1f, 0.6f, 0f);

    [Header("Centro - Minigame da faixa")]
    public CentroEquilibrioFase3 miniGameCentro;

    [Header("Direita - Quiz de programação")]
    public Text txtPerguntaDireita;
    public Button btnDir1;
    public Button btnDir2;
    public Button btnDir3;

    private bool jogoEncerrado = false;

    private int respostaCorretaEsquerda = 0; // 0 = círculo, 1 = quadrado, 2 = triângulo
    private int ultimoIndiceEsquerda = -1;
    private readonly string[] nomesEsquerda = { "CÍRCULO", "QUADRADO", "TRIÂNGULO" };

    [System.Serializable]
    public class PerguntaProgramacao
    {
        public string pergunta;
        public string opcao1;
        public string opcao2;
        public string opcao3;
        public int correta;
    }

    private PerguntaProgramacao perguntaAtualDireita;
    private readonly List<PerguntaProgramacao> perguntasProgramacao = new List<PerguntaProgramacao>();

    void Start()
    {
        if (GameManagerGlobal.Instance != null)
        {
            GameManagerGlobal.Instance.DefinirNivel(3);
        }

        if (txtFeedback != null)
            txtFeedback.text = "";

        if (txtContagem != null)
            txtContagem.text = "";

        MontarPerguntasProgramacao();
        ConfigurarBotoes();
        GerarDesafioEsquerda();
        GerarPerguntaDireita();

        if (miniGameCentro != null)
        {
            miniGameCentro.ReiniciarMarcador();
            miniGameCentro.enabled = false;
        }

        BloquearBotoes(true);

        AtualizarUI();

        StartCoroutine(ContagemInicial());
    }

    void Update()
    {
        if (jogoEncerrado || !faseIniciada) return;

        tempoRestante -= Time.deltaTime;

        if (tempoRestante <= 0f)
        {
            tempoRestante = 0f;
            EncerrarFase();
            return;
        }

        AtualizarUI();
    }

    void ConfigurarBotoes()
    {
        if (btnEsq1 != null)
        {
            btnEsq1.onClick.RemoveAllListeners();
            btnEsq1.onClick.AddListener(ClicarEsq1);
            btnEsq1.GetComponentInChildren<Text>().text = "";
        }

        if (btnEsq2 != null)
        {
            btnEsq2.onClick.RemoveAllListeners();
            btnEsq2.onClick.AddListener(ClicarEsq2);
            btnEsq2.GetComponentInChildren<Text>().text = "";
        }

        if (btnEsq3 != null)
        {
            btnEsq3.onClick.RemoveAllListeners();
            btnEsq3.onClick.AddListener(ClicarEsq3);
            btnEsq3.GetComponentInChildren<Text>().text = "";
        }

        if (btnDir1 != null)
        {
            btnDir1.onClick.RemoveAllListeners();
            btnDir1.onClick.AddListener(ClicarDir1);
        }

        if (btnDir2 != null)
        {
            btnDir2.onClick.RemoveAllListeners();
            btnDir2.onClick.AddListener(ClicarDir2);
        }

        if (btnDir3 != null)
        {
            btnDir3.onClick.RemoveAllListeners();
            btnDir3.onClick.AddListener(ClicarDir3);
        }
    }

    void MontarPerguntasProgramacao()
    {
        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "JavaScript: qual palavra declara uma variável?",
            opcao1 = "var",
            opcao2 = "make",
            opcao3 = "set",
            correta = 1
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Python: qual símbolo inicia um comentário?",
            opcao1 = "//",
            opcao2 = "#",
            opcao3 = "/*",
            correta = 2
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Java: qual palavra cria um objeto?",
            opcao1 = "class",
            opcao2 = "create",
            opcao3 = "new",
            correta = 3
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "C++: qual comando encerra a função main corretamente?",
            opcao1 = "return 0;",
            opcao2 = "break;",
            opcao3 = "end;",
            correta = 1
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "HTML: qual tag cria um link?",
            opcao1 = "<img>",
            opcao2 = "<a>",
            opcao3 = "<div>",
            correta = 2
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "CSS: qual propriedade altera a cor do texto?",
            opcao1 = "font-color",
            opcao2 = "text-color",
            opcao3 = "color",
            correta = 3
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Python: qual função lê dados do teclado?",
            opcao1 = "input",
            opcao2 = "scan",
            opcao3 = "read",
            correta = 1
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Java: qual tipo armazena números inteiros?",
            opcao1 = "float",
            opcao2 = "int",
            opcao3 = "String",
            correta = 2
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "C#: qual símbolo termina uma instrução?",
            opcao1 = ";",
            opcao2 = ":",
            opcao3 = ".",
            correta = 1
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "SQL: qual comando busca dados?",
            opcao1 = "GET",
            opcao2 = "SELECT",
            opcao3 = "OPEN",
            correta = 2
        });
    }

    void GerarDesafioEsquerda()
    {
        int novoIndice = Random.Range(0, nomesEsquerda.Length);

        while (novoIndice == ultimoIndiceEsquerda)
        {
            novoIndice = Random.Range(0, nomesEsquerda.Length);
        }

        respostaCorretaEsquerda = novoIndice;
        ultimoIndiceEsquerda = novoIndice;

        if (txtInstrucaoEsquerda != null)
            txtInstrucaoEsquerda.text = "Clique no " + nomesEsquerda[respostaCorretaEsquerda];

        LimparTextoBotao(btnEsq1);
        LimparTextoBotao(btnEsq2);
        LimparTextoBotao(btnEsq3);
    }

    void LimparTextoBotao(Button botao)
    {
        if (botao == null) return;

        Text texto = botao.GetComponentInChildren<Text>();
        if (texto != null)
            texto.text = "";
    }

    void VerificarEsquerda(int resposta)
    {
        if (jogoEncerrado) return;

        if (resposta == respostaCorretaEsquerda)
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase3SimbolosAcerto();

            MostrarFeedback("Acertou!", Color.green);
        }
        else
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase3SimbolosErro();

            MostrarFeedback("Errou!", corErroAcessivel);
        }

        GerarDesafioEsquerda();
    }

    public void ClicarEsq1() { VerificarEsquerda(0); }
    public void ClicarEsq2() { VerificarEsquerda(1); }
    public void ClicarEsq3() { VerificarEsquerda(2); }

    void GerarPerguntaDireita()
    {
        if (perguntasProgramacao.Count == 0)
            return;

        perguntaAtualDireita = perguntasProgramacao[Random.Range(0, perguntasProgramacao.Count)];

        if (txtPerguntaDireita != null)
            txtPerguntaDireita.text = perguntaAtualDireita.pergunta;

        if (btnDir1 != null)
            btnDir1.GetComponentInChildren<Text>().text = perguntaAtualDireita.opcao1;

        if (btnDir2 != null)
            btnDir2.GetComponentInChildren<Text>().text = perguntaAtualDireita.opcao2;

        if (btnDir3 != null)
            btnDir3.GetComponentInChildren<Text>().text = perguntaAtualDireita.opcao3;
    }

    void VerificarDireita(int opcaoSelecionada)
    {
        if (jogoEncerrado) return;

        if (opcaoSelecionada == perguntaAtualDireita.correta)
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase3QuizAcerto();

            MostrarFeedback("Acertou!", Color.green);
        }
        else
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase3QuizErro();

            MostrarFeedback("Errou!", corErroAcessivel);
        }

        GerarPerguntaDireita();
    }

    public void ClicarDir1() { VerificarDireita(1); }
    public void ClicarDir2() { VerificarDireita(2); }
    public void ClicarDir3() { VerificarDireita(3); }

    void AtualizarUI()
    {
        if (txtTempo != null)
            txtTempo.text = "Tempo: " + Mathf.CeilToInt(tempoRestante).ToString();
    }

    void MostrarFeedback(string mensagem, Color cor)
    {
        if (txtFeedback == null)
            return;

        if (rotinaFeedback != null)
            StopCoroutine(rotinaFeedback);

        rotinaFeedback = StartCoroutine(FeedbackTemporario(mensagem, cor));
    }

    public void MostrarFeedbackAcerto()
    {
        MostrarFeedback("Acertou!", Color.green);
    }

    public void MostrarFeedbackErro()
    {
        MostrarFeedback("Errou!", corErroAcessivel);
    }

    IEnumerator FeedbackTemporario(string mensagem, Color cor)
    {
        txtFeedback.text = mensagem;
        txtFeedback.color = cor;

        yield return new WaitForSeconds(tempoFeedback);

        txtFeedback.text = "";
    }
    IEnumerator ContagemInicial()
    {
        faseIniciada = false;

        if (txtContagem != null)
        {
            txtContagem.text = "3";
            yield return new WaitForSeconds(1f);

            txtContagem.text = "2";
            yield return new WaitForSeconds(1f);

            txtContagem.text = "1";
            yield return new WaitForSeconds(1f);

            txtContagem.text = "Começar!";
            yield return new WaitForSeconds(0.7f);

            txtContagem.text = "";
        }

        faseIniciada = true;

        BloquearBotoes(false);

        if (miniGameCentro != null)
            miniGameCentro.enabled = true;
    }

    void BloquearBotoes(bool bloquear)
    {
        if (btnEsq1 != null) btnEsq1.interactable = !bloquear;
        if (btnEsq2 != null) btnEsq2.interactable = !bloquear;
        if (btnEsq3 != null) btnEsq3.interactable = !bloquear;

        if (btnDir1 != null) btnDir1.interactable = !bloquear;
        if (btnDir2 != null) btnDir2.interactable = !bloquear;
        if (btnDir3 != null) btnDir3.interactable = !bloquear;
    }

    void EncerrarFase()
    {
        jogoEncerrado = true;

        if (GameManagerGlobal.Instance != null)
            GameManagerGlobal.Instance.FinalizarJogo();

        SceneManager.LoadScene("Instrucoes4");
    }
}