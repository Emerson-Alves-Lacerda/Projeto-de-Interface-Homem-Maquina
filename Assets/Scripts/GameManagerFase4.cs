using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManagerFase4 : MonoBehaviour
{
    [Header("Tempo")]
    public float tempoRestante = 45f;

    [Header("Cores de feedback")]
    public Color corErroAcessivel = new Color(1f, 0.6f, 0f);

    [Header("UI Geral")]
    public Text txtTempo;
    public Text txtFeedbackMatematica;
    public Text txtFeedbackCirculo;
    public Text txtContagem;
    public float tempoFeedback = 0.8f;

    private Coroutine rotinaFeedbackMatematica = null;
    private Coroutine rotinaFeedbackCirculo = null;
    private bool faseIniciada = false;

    [Header("1 - Matemática")]
    public Text txtPerguntaMatematica;
    public Button btnMat1;
    public Button btnMat2;
    public Button btnMat3;
    public Text txtStatusMatematica;

    [Header("2 - Barra de Estabilidade")]
    public BarraEstabilidadeFase4 barraEstabilidade;

    [Header("3 - Ritmo")]
    public RitmoFase4 ritmoFase4;

    [Header("4 - Clique no círculo")]
    public GameObject circuloPrefab;
    public RectTransform areaSpawnCirculo;
    public float intervaloSpawnCirculo = 1.2f;
    public float tempoVidaCirculo = 2.5f;
    public int maxCirculosAtivos = 3;
    public Text txtStatusCirculo;

    private int numero1;
    private int numero2;
    private int respostaCorretaMatematica;

    private float tempoSpawnCirculo = 0f;
    private readonly List<GameObject> circulosAtivos = new List<GameObject>();

    private bool jogoEncerrado = false;

    void Start()
    {
        if (GameManagerGlobal.Instance != null)
        {
            GameManagerGlobal.Instance.DefinirNivel(4);
        }

        if (txtFeedbackMatematica != null)
            txtFeedbackMatematica.text = "";

        if (txtFeedbackCirculo != null)
            txtFeedbackCirculo.text = "";

        if (txtContagem != null)
            txtContagem.text = "";

        ConfigurarBotoesMatematica();
        GerarPerguntaMatematica();

        if (barraEstabilidade != null)
        {
            barraEstabilidade.Reiniciar();
            barraEstabilidade.enabled = false;
        }

        if (ritmoFase4 != null)
        {
            ritmoFase4.Reiniciar();
            ritmoFase4.enabled = false;
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

        tempoSpawnCirculo += Time.deltaTime;

        if (tempoSpawnCirculo >= intervaloSpawnCirculo)
        {
            tempoSpawnCirculo = 0f;
            SpawnCirculo();
        }

        LimparCirculosMortos();
        AtualizarUI();
    }

    void ConfigurarBotoesMatematica()
    {
        if (btnMat1 != null)
        {
            btnMat1.onClick.RemoveAllListeners();
            btnMat1.onClick.AddListener(ClicarMat1);
        }

        if (btnMat2 != null)
        {
            btnMat2.onClick.RemoveAllListeners();
            btnMat2.onClick.AddListener(ClicarMat2);
        }

        if (btnMat3 != null)
        {
            btnMat3.onClick.RemoveAllListeners();
            btnMat3.onClick.AddListener(ClicarMat3);
        }
    }

    void GerarPerguntaMatematica()
    {
        int operacao = Random.Range(0, 4);
        string textoOperacao = "";

        switch (operacao)
        {
            case 0:
                numero1 = Random.Range(1, 10);
                numero2 = Random.Range(1, 10);
                respostaCorretaMatematica = numero1 + numero2;
                textoOperacao = numero1 + " + " + numero2;
                break;

            case 1:
                numero1 = Random.Range(5, 15);
                numero2 = Random.Range(1, numero1);
                respostaCorretaMatematica = numero1 - numero2;
                textoOperacao = numero1 + " - " + numero2;
                break;

            case 2:
                numero1 = Random.Range(1, 10);
                numero2 = Random.Range(1, 10);
                respostaCorretaMatematica = numero1 * numero2;
                textoOperacao = numero1 + " x " + numero2;
                break;

            case 3:
                numero2 = Random.Range(1, 10);
                int resultado = Random.Range(1, 10);
                numero1 = numero2 * resultado;
                respostaCorretaMatematica = resultado;
                textoOperacao = numero1 + " ÷ " + numero2;
                break;
        }

        if (txtPerguntaMatematica != null)
            txtPerguntaMatematica.text = "Quanto é " + textoOperacao + "?";

        GerarAlternativasMatematica();
    }

    void GerarAlternativasMatematica()
    {
        int errada1 = respostaCorretaMatematica + Random.Range(1, 5);
        int errada2 = respostaCorretaMatematica - Random.Range(1, 5);

        if (errada2 < 0)
            errada2 = respostaCorretaMatematica + Random.Range(5, 9);

        while (errada1 == respostaCorretaMatematica)
            errada1++;

        while (errada2 == respostaCorretaMatematica || errada2 == errada1)
            errada2++;

        int[] alternativas = new int[3];
        alternativas[0] = respostaCorretaMatematica;
        alternativas[1] = errada1;
        alternativas[2] = errada2;

        Embaralhar(alternativas);

        AplicarTextoBotao(btnMat1, alternativas[0]);
        AplicarTextoBotao(btnMat2, alternativas[1]);
        AplicarTextoBotao(btnMat3, alternativas[2]);
    }

    void Embaralhar(int[] valores)
    {
        for (int i = 0; i < valores.Length; i++)
        {
            int aleatorio = Random.Range(i, valores.Length);

            int temp = valores[i];
            valores[i] = valores[aleatorio];
            valores[aleatorio] = temp;
        }
    }

    void AplicarTextoBotao(Button botao, int valor)
    {
        if (botao == null) return;

        Text txt = botao.GetComponentInChildren<Text>();
        if (txt != null)
            txt.text = valor.ToString();
    }

    void VerificarMatematica(Button botao)
    {
        if (jogoEncerrado || botao == null) return;

        Text txt = botao.GetComponentInChildren<Text>();
        if (txt == null) return;

        int respostaUsuario;

        if (int.TryParse(txt.text, out respostaUsuario))
        {
            if (respostaUsuario == respostaCorretaMatematica)
            {
                if (GameManagerGlobal.Instance != null)
                    GameManagerGlobal.Instance.RegistrarFase4MatematicaAcerto();

                MostrarFeedbackMatematica("Acertou!", Color.green);

                if (txtStatusMatematica != null)
                    txtStatusMatematica.text = "Certo!";
            }
            else
            {
                if (GameManagerGlobal.Instance != null)
                    GameManagerGlobal.Instance.RegistrarFase4MatematicaErro();

                MostrarFeedbackMatematica("Errou!", corErroAcessivel);

                if (txtStatusMatematica != null)
                    txtStatusMatematica.text = "Errado!";
            }

            GerarPerguntaMatematica();
        }
    }

    public void ClicarMat1()
    {
        VerificarMatematica(btnMat1);
    }

    public void ClicarMat2()
    {
        VerificarMatematica(btnMat2);
    }

    public void ClicarMat3()
    {
        VerificarMatematica(btnMat3);
    }

    void SpawnCirculo()
    {
        if (circuloPrefab == null || areaSpawnCirculo == null)
            return;

        if (circulosAtivos.Count >= maxCirculosAtivos)
            return;

        GameObject circulo = Instantiate(circuloPrefab) as GameObject;
        circulo.transform.SetParent(areaSpawnCirculo, false);

        RectTransform rt = circulo.GetComponent<RectTransform>();
        if (rt != null)
        {
            float largura = areaSpawnCirculo.rect.width;
            float altura = areaSpawnCirculo.rect.height;
            float margem = 40f;

            float x = Random.Range(-largura / 2f + margem, largura / 2f - margem);
            float y = Random.Range(-altura / 2f + margem, altura / 2f - margem);

            rt.anchoredPosition = new Vector2(x, y);
            rt.localScale = Vector3.one;
        }

        TargetCliqueFase4 alvo = circulo.GetComponent<TargetCliqueFase4>();
        if (alvo != null)
        {
            alvo.Configurar(this, tempoVidaCirculo);
        }

        circulosAtivos.Add(circulo);
    }

    public void AcertouCirculo()
    {
        if (jogoEncerrado) return;

        if (GameManagerGlobal.Instance != null)
            GameManagerGlobal.Instance.RegistrarFase4CirculoAcerto();

        MostrarFeedbackCirculo("Acertou!", Color.green);

        if (txtStatusCirculo != null)
            txtStatusCirculo.text = "Certo!";
    }

    public void ErrouCirculo()
    {
        if (jogoEncerrado) return;

        if (GameManagerGlobal.Instance != null)
            GameManagerGlobal.Instance.RegistrarFase4CirculoErro();

        MostrarFeedbackCirculo("Errou!", corErroAcessivel);

        if (txtStatusCirculo != null)
            txtStatusCirculo.text = "Perdeu o círculo!";
    }

    void LimparCirculosMortos()
    {
        circulosAtivos.RemoveAll(item => item == null);
    }

    void AtualizarUI()
    {
        if (txtTempo != null)
            txtTempo.text = "Tempo: " + Mathf.CeilToInt(tempoRestante).ToString();
    }

    void MostrarFeedbackMatematica(string mensagem, Color cor)
    {
        if (txtFeedbackMatematica == null)
            return;

        if (rotinaFeedbackMatematica != null)
            StopCoroutine(rotinaFeedbackMatematica);

        rotinaFeedbackMatematica = StartCoroutine(FeedbackTemporario(txtFeedbackMatematica, mensagem, cor));
    }

    void MostrarFeedbackCirculo(string mensagem, Color cor)
    {
        if (txtFeedbackCirculo == null)
            return;

        if (rotinaFeedbackCirculo != null)
            StopCoroutine(rotinaFeedbackCirculo);

        rotinaFeedbackCirculo = StartCoroutine(FeedbackTemporario(txtFeedbackCirculo, mensagem, cor));
    }

    IEnumerator FeedbackTemporario(Text texto, string mensagem, Color cor)
    {
        texto.text = mensagem;
        texto.color = cor;

        yield return new WaitForSeconds(tempoFeedback);

        texto.text = "";
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

        if (barraEstabilidade != null)
            barraEstabilidade.enabled = true;

        if (ritmoFase4 != null)
            ritmoFase4.enabled = true;
    }

    void BloquearBotoes(bool bloquear)
    {
        if (btnMat1 != null) btnMat1.interactable = !bloquear;
        if (btnMat2 != null) btnMat2.interactable = !bloquear;
        if (btnMat3 != null) btnMat3.interactable = !bloquear;
    }

    void EncerrarFase()
    {
        jogoEncerrado = true;

        if (GameManagerGlobal.Instance != null)
            GameManagerGlobal.Instance.FinalizarJogo();

        SceneManager.LoadScene("Resultado");
    }
}