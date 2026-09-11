using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void IniciarJogo()
    {
        if (GameManagerGlobal.Instance != null)
        {
            GameManagerGlobal.Instance.IniciarJogo();
        }

        SceneManager.LoadScene("Instrucoes");
    }

    public void AbrirInstrucoes()
    {
        SceneManager.LoadScene("Instrucoes");
    }

    public void SairJogo()
    {
        Application.Quit();
        Debug.Log("Saiu do jogo");
    }
}
