using UnityEngine;
using UnityEngine.UI;

public class BarraEstabilidadeFase4 : MonoBehaviour
{
    public RectTransform barraArea;
    public RectTransform marcador;
    public RectTransform zonaSegura;

    public Text txtStatus;
    public Color corErroAcessivel = new Color(1f, 0.6f, 0f);

    public float velocidade = 220f;
    public float driftAutomatico = 90f;
    public float intervaloAcerto = 1.5f;

    private float direcao = 1f;
    private float tempoAcerto = 0f;

    void Start()
    {
        Reiniciar();
    }

    void Update()
    {
        if (marcador == null || barraArea == null || zonaSegura == null)
            return;

        float input = Input.GetAxis("Horizontal");

        Vector2 pos = marcador.anchoredPosition;

        pos.x += input * velocidade * Time.deltaTime;
        pos.x += direcao * driftAutomatico * Time.deltaTime;

        if (Random.Range(0f, 1f) < 0.02f)
        {
            direcao *= -1f;
        }

        marcador.anchoredPosition = pos;

        if (SaiuDaBarra())
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarFase4RastreamentoErro();

            if (txtStatus != null)
            {
                txtStatus.text = "Saiu!";
                txtStatus.color = corErroAcessivel;
            }

            Reiniciar();
            return;
        }

        tempoAcerto += Time.deltaTime;

        if (tempoAcerto >= intervaloAcerto)
        {
            tempoAcerto = 0f;

            if (DentroDaZona())
            {
                if (GameManagerGlobal.Instance != null)
                    GameManagerGlobal.Instance.RegistrarFase4RastreamentoAcerto();

                if (txtStatus != null)
                {
                    txtStatus.text = "Estável!";
                    txtStatus.color = Color.green;
                }
            }
            else
            {
                if (GameManagerGlobal.Instance != null)
                    GameManagerGlobal.Instance.RegistrarFase4RastreamentoErro();

                if (txtStatus != null)
                {
                    txtStatus.text = "Instável!";
                    txtStatus.color = corErroAcessivel;
                }
            }
        }
    }

    bool DentroDaZona()
    {
        float marcadorX = marcador.anchoredPosition.x;

        float esquerda =
            zonaSegura.anchoredPosition.x -
            (zonaSegura.rect.width / 2f);

        float direita =
            zonaSegura.anchoredPosition.x +
            (zonaSegura.rect.width / 2f);

        return marcadorX >= esquerda && marcadorX <= direita;
    }

    bool SaiuDaBarra()
    {
        float largura = barraArea.rect.width;
        float marcadorLargura = marcador.rect.width;

        float esquerda = -(largura / 2f) + marcadorLargura / 2f;
        float direita = (largura / 2f) - marcadorLargura / 2f;

        float x = marcador.anchoredPosition.x;

        return x < esquerda || x > direita;
    }

    public void Reiniciar()
    {
        if (marcador != null)
            marcador.anchoredPosition = Vector2.zero;

        direcao = Random.value < 0.5f ? -1f : 1f;

        tempoAcerto = 0f;

        if (txtStatus != null)
        {
            txtStatus.text = "";
            txtStatus.color = Color.white;
        }
    }
}