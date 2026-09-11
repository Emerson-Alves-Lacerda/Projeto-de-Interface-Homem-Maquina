using UnityEngine;
using UnityEngine.UI;

public class RitmoFase4 : MonoBehaviour
{
    public RectTransform marcador;
    public Button btnClique;
    public Text txtStatus;
    public Color corErroAcessivel = new Color(1f, 0.6f, 0f);

    public float velocidade = 240f;
    public float limiteEsquerda = -280f;
    public float limiteDireita = 280f;
    public float zonaCentralEsquerda = -50f;
    public float zonaCentralDireita = 50f;

    private float direcao = 1f;
    private bool bloqueado = false;

    void Start()
    {
        if (btnClique != null)
        {
            btnClique.onClick.RemoveAllListeners();
            btnClique.onClick.AddListener(RegistrarClique);
        }

        Reiniciar();
    }

    void Update()
    {
        if (bloqueado || marcador == null)
            return;

        Vector2 pos = marcador.anchoredPosition;
        pos.x += direcao * velocidade * Time.deltaTime;

        if (pos.x >= limiteDireita)
        {
            pos.x = limiteDireita;
            direcao = -1f;
        }
        else if (pos.x <= limiteEsquerda)
        {
            pos.x = limiteEsquerda;
            direcao = 1f;
        }

        marcador.anchoredPosition = pos;
    }

    public void RegistrarClique()
    {
        if (bloqueado || marcador == null)
            return;

        bool acertou =
            marcador.anchoredPosition.x >= zonaCentralEsquerda &&
            marcador.anchoredPosition.x <= zonaCentralDireita;

        if (GameManagerGlobal.Instance != null)
        {
            if (acertou)
                GameManagerGlobal.Instance.RegistrarFase4RitmoAcerto();
            else
                GameManagerGlobal.Instance.RegistrarFase4RitmoErro();
        }

        if (txtStatus != null)
        {
            if (acertou)
            {
                txtStatus.text = "No tempo!";
                txtStatus.color = Color.green;
            }
            else
            {
                txtStatus.text = "Fora do tempo!";
                txtStatus.color = corErroAcessivel;
            }
        }
    }

    public void Reiniciar()
    {
        if (marcador != null)
            marcador.anchoredPosition = Vector2.zero;

        direcao = Random.value < 0.5f ? -1f : 1f;

        if (txtStatus != null)
        {
            txtStatus.text = "";
            txtStatus.color = Color.white;
        }
    }
}