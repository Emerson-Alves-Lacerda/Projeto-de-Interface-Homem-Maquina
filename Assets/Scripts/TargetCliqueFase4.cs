using UnityEngine;
using UnityEngine.EventSystems;

public class TargetCliqueFase4 : MonoBehaviour, IPointerClickHandler
{
    private GameManagerFase4 gameManager;
    private float tempoVida = 2.5f;
    private bool resolvido = false;

    public void Configurar(GameManagerFase4 manager, float vida)
    {
        gameManager = manager;
        tempoVida = vida;
    }

    void Update()
    {
        if (resolvido) return;

        tempoVida -= Time.deltaTime;

        if (tempoVida <= 0f)
        {
            resolvido = true;

            if (gameManager != null)
                gameManager.ErrouCirculo();

            Destroy(gameObject);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (resolvido) return;

        resolvido = true;

        if (gameManager != null)
            gameManager.AcertouCirculo();

        Destroy(gameObject);
    }
}