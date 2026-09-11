using UnityEngine;

public class BalanceTheBallFase3 : MonoBehaviour
{
    public Transform bola;
    public GameManagerFase3 gameManager;

    public float velocidadeMovimento = 4f;
    public float driftAutomatico = 0.7f;
    public float limiteEsquerda = -3f;
    public float limiteDireita = 3f;

    private float direcaoDrift = 1f;
    private bool bloqueado = false;

    void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManagerFase3>();

        ReiniciarBola();
    }

    void Update()
    {
        if (bola == null || bloqueado) return;

        float input = Input.GetAxis("Horizontal");

        Vector3 pos = bola.localPosition;
        pos.x += input * velocidadeMovimento * Time.deltaTime;
        pos.x += direcaoDrift * driftAutomatico * Time.deltaTime;

        if (Random.Range(0f, 1f) < 0.015f)
        {
            direcaoDrift *= -1f;
        }

        if (pos.x < limiteEsquerda || pos.x > limiteDireita)
        {
            PossoPerder();
            return;
        }

        bola.localPosition = pos;
    }

    void PossoPerder()
    {
        if (GameManagerGlobal.Instance != null)
        {
            GameManagerGlobal.Instance.RegistrarErro();
        }

        bloqueado = true;
        ReiniciarBola();
        Invoke("Desbloquear", 0.5f);
    }

    void Desbloquear()
    {
        bloqueado = false;
    }

    public void ReiniciarBola()
    {
        if (bola != null)
            bola.localPosition = Vector3.zero;
    }
}