using UnityEngine;
using UnityEngine.UI;

public class CentroEquilibrioFase3 : MonoBehaviour
{
    public RectTransform painelCentro;
    public RectTransform marcador;
    public Text txtInstrucaoCentro;
    public Text txtStatusCentro;
    public GameManagerFase3 gameManager;

    public float velocidadeMovimento = 220f;
    public float driftAutomatico = 75f;
    public float zonaSeguraEsquerda = -80f;
    public float zonaSeguraDireita = 80f;
    public float intervaloVerificacao = 1.5f;

    private float direcaoDrift = 1f;
    private float tempoVerificacao = 0f;
    private bool bloqueado = false;

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManagerFase3>();

        if (txtInstrucaoCentro != null)
            txtInstrucaoCentro.text = "Mantenha o marcador no centro";

        if (txtStatusCentro != null)
            txtStatusCentro.text = "";

        ReiniciarMarcador();
    }

    void Update()
    {
        if (bloqueado || marcador == null || painelCentro == null)
            return;

        float input = Input.GetAxis("Horizontal");

        Vector2 pos = marcador.anchoredPosition;
        pos.x += input * velocidadeMovimento * Time.deltaTime;
        pos.x += direcaoDrift * driftAutomatico * Time.deltaTime;

        if (Random.Range(0f, 1f) < 0.02f)
        {
            direcaoDrift *= -1f;
        }

        marcador.anchoredPosition = pos;

        if (SaiuDoPanel())
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase3CentroErro();

            if (gameManager != null)
                gameManager.MostrarFeedbackErro();

            if (txtStatusCentro != null)
                txtStatusCentro.text = "Saiu da faixa!";

            ReiniciarMarcador();
            return;
        }

        tempoVerificacao += Time.deltaTime;

        if (tempoVerificacao >= intervaloVerificacao)
        {
            tempoVerificacao = 0f;
            VerificarFaixaSegura();
        }
    }

    void VerificarFaixaSegura()
    {
        bool dentroDaFaixa =
            marcador.anchoredPosition.x >= zonaSeguraEsquerda &&
            marcador.anchoredPosition.x <= zonaSeguraDireita;

        if (dentroDaFaixa)
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase3CentroAcerto();

            if (txtStatusCentro != null)
                txtStatusCentro.text = "Dentro da faixa.";
        }
    }

    bool SaiuDoPanel()
    {
        float larguraPainel = painelCentro.rect.width;
        float larguraMarcador = marcador.rect.width;

        float limiteEsquerda = -(larguraPainel / 2f) + (larguraMarcador / 2f);
        float limiteDireita = (larguraPainel / 2f) - (larguraMarcador / 2f);

        float x = marcador.anchoredPosition.x;

        return x < limiteEsquerda || x > limiteDireita;
    }

    public void ReiniciarMarcador()
    {
        if (marcador != null)
            marcador.anchoredPosition = Vector2.zero;

        tempoVerificacao = 0f;
        direcaoDrift = Random.value < 0.5f ? -1f : 1f;

        if (txtStatusCentro != null)
            txtStatusCentro.text = "Dentro da faixa.";
    }

    public void Bloquear()
    {
        bloqueado = true;
    }

    public void Desbloquear()
    {
        bloqueado = false;
        ReiniciarMarcador();
    }
}