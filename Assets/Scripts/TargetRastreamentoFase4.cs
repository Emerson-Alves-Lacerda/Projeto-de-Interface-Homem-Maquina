using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TargetRastreamentoFase4 : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform areaMovimento;
    public Text txtStatus;
    public float intervaloAvaliar = 1.2f;
    public float velocidade = 500f;

    private RectTransform rt;
    private Vector2 destino;
    private float tempo = 0f;
    private bool sobCursor = false;

    void Start()
    {
        rt = GetComponent<RectTransform>();

        if (areaMovimento == null)
            areaMovimento = transform.parent as RectTransform;

        GerarDestino();
    }

    void Update()
    {
        if (rt == null || areaMovimento == null)
            return;

        rt.anchoredPosition = Vector2.MoveTowards(rt.anchoredPosition, destino, velocidade * Time.deltaTime);

        if (Vector2.Distance(rt.anchoredPosition, destino) < 5f)
        {
            GerarDestino();
        }

        tempo += Time.deltaTime;

        if (tempo >= intervaloAvaliar)
        {
            tempo = 0f;
            Avaliar();
        }
    }

    void Avaliar()
    {
        if (sobCursor)
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarAcerto();

            if (txtStatus != null)
                txtStatus.text = "Alvo rastreado!";
        }
        else
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarErro();

            if (txtStatus != null)
                txtStatus.text = "Perdeu o alvo!";
        }
    }

    void GerarDestino()
    {
        if (areaMovimento == null || rt == null)
            return;

        float largura = areaMovimento.rect.width;
        float altura = areaMovimento.rect.height;
        float margem = 40f;

        float x = Random.Range(-largura / 2f + margem, largura / 2f - margem);
        float y = Random.Range(-altura / 2f + margem, altura / 2f - margem);

        destino = new Vector2(x, y);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        sobCursor = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        sobCursor = false;
    }
}