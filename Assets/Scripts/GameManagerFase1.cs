using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManagerFase1 : MonoBehaviour
{
    private enum CondicaoCognitiva
    {
        BaixaCarga,
        AltaCarga
    }

    [Header("Prefabs")]
    public GameObject alvoPrefab;       // amarelo / círculo
    public GameObject distratorPrefab;  // azul escuro / outra forma

    [Header("UI")]
    public Text txtTempo;
    public Text txtFeedback;
    public Text txtContagem;
    public float tempoFeedback = 0.8f;

    private Coroutine rotinaFeedback;

    [Header("Tempo")]
    public float tempoBaixaCarga = 5f;
    public float tempoAltaCarga = 10f;

    public float pausaEntreCondicoes = 1.5f;

    [Header("Quantidade de alvos")]
    public int amarelosBaixa = 2;
    public int distratoresBaixa = 6;

    public int amarelosAlta = 8;
    public int distratoresAlta = 6;

    [Header("Área de spawn")]
    public float minX = -7f;
    public float maxX = 7f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Visual")]
    public Color corAmareloBaixa = Color.yellow;
    public Color corAmareloAlta = new Color(1f, 0.85f, 0.2f);
    public Color corAzulEscuro = new Color(0f, 0f, 0.45f);

    [Header("Cores de feedback")]
    public Color corErroAcessivel = new Color(1f, 0.6f, 0f);

    public float escalaBaixa = 1f;
    public float escalaAlta = 0.85f;

    private int pontos = 0;
    private float tempoAtual;
    private bool jogoEncerrado = false;
    private bool transicaoEmAndamento = false;
    private bool faseIniciada = false;

    private CondicaoCognitiva condicaoAtual;

    private int acertosCondicao;
    private int errosCondicao;

    private int acertosBaixa;
    private int errosBaixa;

    private int acertosAlta;
    private int errosAlta;

    private float somaTempoCliquesBaixa = 0f;
    private int quantidadeCliquesBaixa = 0;

    private float somaTempoCliquesAlta = 0f;
    private int quantidadeCliquesAlta = 0;

    private readonly List<GameObject> alvosAtivos = new List<GameObject>();

    void Start()
    {
        if (GameManagerGlobal.Instance != null)
        {
            GameManagerGlobal.Instance.DefinirNivel(1);
        }

        if (txtFeedback != null)
            txtFeedback.text = "";

        if (txtContagem != null)
            txtContagem.text = "";

        if (txtTempo != null)
            txtTempo.text = "Tempo: --";

        pontos = 0;
        faseIniciada = false;

        StartCoroutine(ContagemInicial());
    }

    void Update()
    {
        if (jogoEncerrado || transicaoEmAndamento || !faseIniciada)
            return;

        tempoAtual -= Time.deltaTime;

        if (tempoAtual <= 0f)
        {
            tempoAtual = 0f;
            FinalizarCondicaoAtual();
            return;
        }

        AtualizarUI();
    }

    void IniciarCondicaoBaixa()
    {
        condicaoAtual = CondicaoCognitiva.BaixaCarga;
        tempoAtual = tempoBaixaCarga;
        acertosCondicao = 0;
        errosCondicao = 0;

        LimparAlvos();
        SpawnLoteAtual();
        AtualizarUI();
    }

    void IniciarCondicaoAlta()
    {
        condicaoAtual = CondicaoCognitiva.AltaCarga;
        tempoAtual = tempoAltaCarga;
        acertosCondicao = 0;
        errosCondicao = 0;

        LimparAlvos();
        SpawnLoteAtual();
        AtualizarUI();
    }

    void SpawnLoteAtual()
    {
        int qtdAmarelos = condicaoAtual == CondicaoCognitiva.BaixaCarga ? amarelosBaixa : amarelosAlta;
        int qtdDistratores = condicaoAtual == CondicaoCognitiva.BaixaCarga ? distratoresBaixa : distratoresAlta;

        Color corAlvo = condicaoAtual == CondicaoCognitiva.BaixaCarga ? corAmareloBaixa : corAmareloAlta;
        float escala = condicaoAtual == CondicaoCognitiva.BaixaCarga ? escalaBaixa : escalaAlta;

        for (int i = 0; i < qtdAmarelos; i++)
        {
            SpawnTarget(false, corAlvo, escala);
        }

        for (int i = 0; i < qtdDistratores; i++)
        {
            SpawnTarget(true, corAzulEscuro, escala);
        }
    }

    void SpawnTarget(bool distrator, Color cor, float escala)
    {
        GameObject prefabUsado = distrator ? distratorPrefab : alvoPrefab;

        if (prefabUsado == null)
            return;

        Vector3 pos = new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            0f
        );

        GameObject alvo = Instantiate(prefabUsado, pos, Quaternion.identity);

        TargetFase1 targetScript = alvo.GetComponent<TargetFase1>();
        if (targetScript != null)
        {
            targetScript.Configurar(distrator, this, cor, escala, Time.time);
        }

        alvosAtivos.Add(alvo);
    }

    public void RegistrarClique(bool clicouEmDistrator, float tempoSpawnAlvo)
    {
        if (jogoEncerrado || transicaoEmAndamento || !faseIniciada)
            return;

        float tempoReacao = Time.time - tempoSpawnAlvo;

        if (!clicouEmDistrator)
        {
            if (condicaoAtual == CondicaoCognitiva.BaixaCarga)
            {
                somaTempoCliquesBaixa += tempoReacao;
                quantidadeCliquesBaixa++;
            }
            else
            {
                somaTempoCliquesAlta += tempoReacao;
                quantidadeCliquesAlta++;
            }
        }

        if (clicouEmDistrator)
        {
            pontos -= 1;
            errosCondicao++;

            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarErro();

            MostrarFeedback("Errou!", corErroAcessivel);
        }
        else
        {
            pontos += 1;
            acertosCondicao++;

            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarAcerto();

            MostrarFeedback("Acertou!", Color.green);
        }

        AtualizarUI();
        RemoverNulosDaLista();

        if (alvosAtivos.Count == 0 && !transicaoEmAndamento)
        {
            SpawnLoteAtual();
        }
    }

    void FinalizarCondicaoAtual()
    {
        transicaoEmAndamento = true;
        LimparAlvos();
        AtualizarUI();

        if (condicaoAtual == CondicaoCognitiva.BaixaCarga)
        {
            acertosBaixa = acertosCondicao;
            errosBaixa = errosCondicao;

            if (GameManagerGlobal.Instance != null)
            {
                GameManagerGlobal.Instance.acertosBaixaCarga = acertosBaixa;
                GameManagerGlobal.Instance.errosBaixaCarga = errosBaixa;
                GameManagerGlobal.Instance.tempoMedioBaixaCarga = GetMediaTempoBaixa();
            }

            Invoke("IniciarAltaCondicaoComSeguranca", pausaEntreCondicoes);
        }
        else
        {
            acertosAlta = acertosCondicao;
            errosAlta = errosCondicao;

            if (GameManagerGlobal.Instance != null)
            {
                GameManagerGlobal.Instance.acertosAltaCarga = acertosAlta;
                GameManagerGlobal.Instance.errosAltaCarga = errosAlta;
                GameManagerGlobal.Instance.tempoMedioAltaCarga = GetMediaTempoAlta();
            }

            Invoke("IrParaInstrucoes2", pausaEntreCondicoes);
        }
    }

    void IniciarAltaCondicaoComSeguranca()
    {
        StartCoroutine(ContagemAltaCarga());
    }

    void IrParaInstrucoes2()
    {
        jogoEncerrado = true;
        SceneManager.LoadScene("Instrucoes2");
    }

    void LimparAlvos()
    {
        for (int i = 0; i < alvosAtivos.Count; i++)
        {
            if (alvosAtivos[i] != null)
                Destroy(alvosAtivos[i]);
        }

        alvosAtivos.Clear();
    }

    void RemoverNulosDaLista()
    {
        alvosAtivos.RemoveAll(item => item == null);
    }

    float GetMediaTempoBaixa()
    {
        if (quantidadeCliquesBaixa == 0)
            return 0f;

        return somaTempoCliquesBaixa / quantidadeCliquesBaixa;
    }

    float GetMediaTempoAlta()
    {
        if (quantidadeCliquesAlta == 0)
            return 0f;

        return somaTempoCliquesAlta / quantidadeCliquesAlta;
    }

    void AtualizarUI()
    {
        if (txtTempo != null)
            txtTempo.text = "Tempo: " + Mathf.CeilToInt(tempoAtual);
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
        transicaoEmAndamento = true;
        faseIniciada = false;

        LimparAlvos();

        if (txtTempo != null)
            txtTempo.text = "Tempo: --";

        yield return StartCoroutine(MostrarContagem());

        faseIniciada = true;
        transicaoEmAndamento = false;

        IniciarCondicaoBaixa();
    }

    IEnumerator ContagemAltaCarga()
    {
        transicaoEmAndamento = true;

        LimparAlvos();

        if (txtTempo != null)
            txtTempo.text = "Tempo: --";

        yield return StartCoroutine(MostrarContagem());

        transicaoEmAndamento = false;

        IniciarCondicaoAlta();
    }

    IEnumerator MostrarContagem()
    {
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
    }
}