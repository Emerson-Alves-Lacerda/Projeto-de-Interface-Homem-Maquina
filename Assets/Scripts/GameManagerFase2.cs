using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManagerFase2 : MonoBehaviour
{
    [Header("UI Geral")]
    public Text txtTempo;
    public Text txtFeedback;
    public Text txtContagem;
    public float tempoFeedback = 0.8f;

    private Coroutine rotinaFeedback = null;
    private bool faseIniciada = false;

    [Header("Esquerda - Memória Visual")]
    public Text txtMemoriaInstrucao;
    public Text txtMemoriaSequencia;
    public Text txtMemoriaStatus;
    public Button btnMemoria1;
    public Button btnMemoria2;
    public Button btnMemoria3;

    [Header("Cores de feedback")]
    public Color corErroAcessivel = new Color(1f, 0.6f, 0f);

    [Header("Direita - Quiz de Programação")]
    public Text txtPerguntaDireita;
    public Button btnDireita1;
    public Button btnDireita2;
    public Button btnDireita3;

    [Header("Tempo")]
    public float tempoRestante = 45f;

    private bool jogoEncerrado = false;

    private readonly string[] opcoesMemoria = { "AMARELO", "AZUL ESCURO", "ROXO" };
    private readonly Color corAmarelo = Color.yellow;
    private readonly Color corAzulEscuro = new Color(0f, 0f, 0.45f);
    private readonly Color corRoxo = new Color(0.55f, 0.2f, 0.75f);

    private string[] sequenciaMemoria = new string[3];
    private int indiceMemoria = 0;
    private bool exibindoSequencia = false;
    private Coroutine rotinaMemoria = null;

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
            GameManagerGlobal.Instance.DefinirNivel(2);
        }

        if (txtFeedback != null)
            txtFeedback.text = "";

        if (txtContagem != null)
            txtContagem.text = "";

        MontarPerguntasProgramacao();
        ConfigurarBotoes();
        GerarPerguntaDireita();

        BloquearBotoes(true);

        AtualizarUI();

        StartCoroutine(ContagemInicial());
    }
    void Update()
    {
        if (jogoEncerrado || !faseIniciada) return;

        tempoRestante -= Time.deltaTime;

        if (tempoRestante <= 0)
        {
            tempoRestante = 0;
            EncerrarFase();
            return;
        }

        AtualizarUI();
    }

    void ConfigurarBotoes()
    {
        if (btnMemoria1 != null)
        {
            btnMemoria1.onClick.RemoveAllListeners();
            btnMemoria1.onClick.AddListener(ClicarMemoria1);
            AplicarEstiloBotao(btnMemoria1, "AMARELO", corAmarelo);
        }

        if (btnMemoria2 != null)
        {
            btnMemoria2.onClick.RemoveAllListeners();
            btnMemoria2.onClick.AddListener(ClicarMemoria2);
            AplicarEstiloBotao(btnMemoria2, "AZUL ESCURO", corAzulEscuro);
        }

        if (btnMemoria3 != null)
        {
            btnMemoria3.onClick.RemoveAllListeners();
            btnMemoria3.onClick.AddListener(ClicarMemoria3);
            AplicarEstiloBotao(btnMemoria3, "ROXO", corRoxo);
        }

        if (btnDireita1 != null)
        {
            btnDireita1.onClick.RemoveAllListeners();
            btnDireita1.onClick.AddListener(ClicarDireita1);
        }

        if (btnDireita2 != null)
        {
            btnDireita2.onClick.RemoveAllListeners();
            btnDireita2.onClick.AddListener(ClicarDireita2);
        }

        if (btnDireita3 != null)
        {
            btnDireita3.onClick.RemoveAllListeners();
            btnDireita3.onClick.AddListener(ClicarDireita3);
        }
    }

    void AplicarEstiloBotao(Button botao, string texto, Color cor)
    {
        Text txt = botao.GetComponentInChildren<Text>();
        if (txt != null)
        {
            txt.text = texto;
            txt.color = cor;
        }

        Image img = botao.GetComponent<Image>();
        if (img != null)
        {
            img.color = new Color(cor.r, cor.g, cor.b, 0.85f);
        }
    }

    void MontarPerguntasProgramacao()
    {
        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Python: qual palavra-chave cria uma função?",
            opcao1 = "func",
            opcao2 = "def",
            opcao3 = "method",
            correta = 2
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Java: qual tipo armazena texto?",
            opcao1 = "String",
            opcao2 = "char",
            opcao3 = "bool",
            correta = 1
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "C++: qual biblioteca é usada para entrada e saída?",
            opcao1 = "math",
            opcao2 = "scanner",
            opcao3 = "iostream",
            correta = 3
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Python: qual estrutura repete enquanto a condição for verdadeira?",
            opcao1 = "repeat",
            opcao2 = "while",
            opcao3 = "loop",
            correta = 2
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Java: qual palavra é usada para declarar uma classe pública?",
            opcao1 = "public class",
            opcao2 = "new class",
            opcao3 = "main class",
            correta = 1
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "C++: qual operador é usado com cout para imprimir?",
            opcao1 = "=>",
            opcao2 = "::",
            opcao3 = "<<",
            correta = 3
        });
        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Python: qual função exibe algo na tela?",
            opcao1 = "print",
            opcao2 = "show",
            opcao3 = "echo",
            correta = 1
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Java: qual método inicia a execução do programa?",
            opcao1 = "start",
            opcao2 = "main",
            opcao3 = "run",
            correta = 2
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "C++: qual símbolo termina uma instrução?",
            opcao1 = ".",
            opcao2 = ",",
            opcao3 = ";",
            correta = 3
        });

        perguntasProgramacao.Add(new PerguntaProgramacao
        {
            pergunta = "Python: qual tipo representa verdadeiro ou falso?",
            opcao1 = "bool",
            opcao2 = "int",
            opcao3 = "str",
            correta = 1
        });
    }

    void GerarPerguntaDireita()
    {
        if (perguntasProgramacao.Count == 0)
            return;

        perguntaAtualDireita = perguntasProgramacao[Random.Range(0, perguntasProgramacao.Count)];

        if (txtPerguntaDireita != null)
            txtPerguntaDireita.text = perguntaAtualDireita.pergunta;

        if (btnDireita1 != null)
            btnDireita1.GetComponentInChildren<Text>().text = perguntaAtualDireita.opcao1;

        if (btnDireita2 != null)
            btnDireita2.GetComponentInChildren<Text>().text = perguntaAtualDireita.opcao2;

        if (btnDireita3 != null)
            btnDireita3.GetComponentInChildren<Text>().text = perguntaAtualDireita.opcao3;
    }

    void VerificarMemoria(string corSelecionada)
    {
        if (jogoEncerrado || !faseIniciada || exibindoSequencia) return;

        if (corSelecionada == sequenciaMemoria[indiceMemoria])
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase2MemoriaAcerto();

            MostrarFeedback("Acertou!", Color.green);

            indiceMemoria++;

            if (indiceMemoria >= sequenciaMemoria.Length)
            {
                if (txtMemoriaStatus != null)
                    txtMemoriaStatus.text = "Sequência correta!";

                IniciarDesafioMemoria();
                return;
            }
            else
            {
                if (txtMemoriaStatus != null)
                    txtMemoriaStatus.text = "Passo " + (indiceMemoria + 1) + "/3";
            }
        }
        else
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase2MemoriaErro();

            MostrarFeedback("Errou!", corErroAcessivel);

            if (txtMemoriaStatus != null)
                txtMemoriaStatus.text = "Sequência incorreta!";

            IniciarDesafioMemoria();
            return;
        }
    }

    void VerificarDireita(int opcaoSelecionada)
    {
        if (jogoEncerrado || !faseIniciada) return;

        if (opcaoSelecionada == perguntaAtualDireita.correta)
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase2QuizAcerto();

            MostrarFeedback("Acertou!", Color.green);
        }
        else
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase2QuizErro();

            MostrarFeedback("Errou!", corErroAcessivel);
        }

        GerarPerguntaDireita();
    }

    public void ClicarMemoria1() { VerificarMemoria("AMARELO"); }
    public void ClicarMemoria2() { VerificarMemoria("AZUL ESCURO"); }
    public void ClicarMemoria3() { VerificarMemoria("ROXO"); }

    public void ClicarDireita1() { VerificarDireita(1); }
    public void ClicarDireita2() { VerificarDireita(2); }
    public void ClicarDireita3() { VerificarDireita(3); }

    void IniciarDesafioMemoria()
    {
        if (rotinaMemoria != null)
        {
            StopCoroutine(rotinaMemoria);
        }

        indiceMemoria = 0;
        GerarSequenciaMemoria();
        rotinaMemoria = StartCoroutine(MostrarSequenciaMemoria());
    }

    void GerarSequenciaMemoria()
    {
        for (int i = 0; i < sequenciaMemoria.Length; i++)
        {
            sequenciaMemoria[i] = opcoesMemoria[Random.Range(0, opcoesMemoria.Length)];
        }
    }

    IEnumerator MostrarSequenciaMemoria()
    {
        exibindoSequencia = true;

        if (txtMemoriaInstrucao != null)
            txtMemoriaInstrucao.text = "Observe a sequência";

        if (txtMemoriaStatus != null)
            txtMemoriaStatus.text = "";

        if (txtMemoriaSequencia != null)
            txtMemoriaSequencia.text = "";

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < sequenciaMemoria.Length; i++)
        {
            if (txtMemoriaSequencia != null)
            {
                txtMemoriaSequencia.text = sequenciaMemoria[i];
                txtMemoriaSequencia.color = CorDaPalavra(sequenciaMemoria[i]);
            }

            yield return new WaitForSeconds(0.8f);

            if (txtMemoriaSequencia != null)
                txtMemoriaSequencia.text = "";

            yield return new WaitForSeconds(0.25f);
        }

        exibindoSequencia = false;

        if (txtMemoriaInstrucao != null)
            txtMemoriaInstrucao.text = "Repita a sequência";

        if (txtMemoriaStatus != null)
            txtMemoriaStatus.text = "Passo 1/3";
    }

    Color CorDaPalavra(string palavra)
    {
        if (palavra == "AMARELO") return corAmarelo;
        if (palavra == "AZUL ESCURO") return corAzulEscuro;
        return corRoxo;
    }

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
            txtContagem.text = "3";

        yield return new WaitForSeconds(1f);

        if (txtContagem != null)
            txtContagem.text = "2";

        yield return new WaitForSeconds(1f);

        if (txtContagem != null)
            txtContagem.text = "1";

        yield return new WaitForSeconds(1f);

        if (txtContagem != null)
            txtContagem.text = "Começar!";

        yield return new WaitForSeconds(0.7f);

        if (txtContagem != null)
            txtContagem.text = "";

        faseIniciada = true;

        BloquearBotoes(false);

        IniciarDesafioMemoria();
    }

    void BloquearBotoes(bool bloquear)
    {
        if (btnMemoria1 != null) btnMemoria1.interactable = !bloquear;
        if (btnMemoria2 != null) btnMemoria2.interactable = !bloquear;
        if (btnMemoria3 != null) btnMemoria3.interactable = !bloquear;

        if (btnDireita1 != null) btnDireita1.interactable = !bloquear;
        if (btnDireita2 != null) btnDireita2.interactable = !bloquear;
        if (btnDireita3 != null) btnDireita3.interactable = !bloquear;
    }

    void EncerrarFase()
    {
        jogoEncerrado = true;
        SceneManager.LoadScene("Instrucoes3");
    }
}