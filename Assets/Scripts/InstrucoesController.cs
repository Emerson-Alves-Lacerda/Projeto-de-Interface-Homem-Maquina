using UnityEngine;
using UnityEngine.SceneManagement;

public class InstrucoesController : MonoBehaviour
{
    public void ComecarJogo()
    {
        SceneManager.LoadScene("Fase1");
    }
}
