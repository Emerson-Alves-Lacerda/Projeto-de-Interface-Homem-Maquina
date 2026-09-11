using UnityEngine;

public class TargetFase1 : MonoBehaviour
{
    private GameManagerFase1 gameManager;
    private bool ehDistrator;
    private float tempoSpawn;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Configurar(bool distrator, GameManagerFase1 manager, Color cor, float escala, float tempoCriacao)
    {
        ehDistrator = distrator;
        gameManager = manager;
        tempoSpawn = tempoCriacao;

        if (spriteRenderer != null)
            spriteRenderer.color = cor;

        transform.localScale = Vector3.one * escala;
    }

    private void OnMouseDown()
    {
        if (gameManager != null)
        {
            gameManager.RegistrarClique(ehDistrator, tempoSpawn);
        }

        Destroy(gameObject);
    }
}