using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultadoController : MonoBehaviour
{
    public Text txtResumoGeral;
    public Text txtFase1;
    public Text txtFase2;
    public Text txtFase3;
    public Text txtFase4;
    public Text txtMensagemFinal;

    void Start()
    {
        AtualizarResultados();
    }

    void AtualizarResultados()
    {
        if (GameManagerGlobal.Instance != null)
        {
            txtResumoGeral.text =
                "RESUMO GERAL\n" +
                "Acertos totais: " + GameManagerGlobal.Instance.acertos + "\n" +
                "Erros totais: " + GameManagerGlobal.Instance.erros + "\n" +
                "Tempo total: " + GameManagerGlobal.Instance.GetTempoDecorrido().ToString("F1") + "s\n" +
                "Nível atingido: " + GameManagerGlobal.Instance.nivel;

            txtFase1.text =
                "FASE 1 - ATENÇÃO SELETIVA\n" +
                "Baixa carga:\n" +
                "Acertos: " + GameManagerGlobal.Instance.acertosBaixaCarga +
                " | Erros: " + GameManagerGlobal.Instance.errosBaixaCarga +
                " | Tempo médio: " + GameManagerGlobal.Instance.tempoMedioBaixaCarga.ToString("F2") + "s\n\n" +
                "Alta carga:\n" +
                "Acertos: " + GameManagerGlobal.Instance.acertosAltaCarga +
                " | Erros: " + GameManagerGlobal.Instance.errosAltaCarga +
                " | Tempo médio: " + GameManagerGlobal.Instance.tempoMedioAltaCarga.ToString("F2") + "s";

            txtFase2.text =
                "FASE 2 - ATENÇÃO DIVIDIDA\n" +
                "Memória visual:\n" +
                "Acertos: " + GameManagerGlobal.Instance.fase2MemoriaAcertos +
                " | Erros: " + GameManagerGlobal.Instance.fase2MemoriaErros + "\n\n" +
                "Quiz de programação:\n" +
                "Acertos: " + GameManagerGlobal.Instance.fase2QuizAcertos +
                " | Erros: " + GameManagerGlobal.Instance.fase2QuizErros;

            txtFase3.text =
                "FASE 3 - SOBRECARGA COGNITIVA\n" +
                "Símbolos:\n" +
                "Acertos: " + GameManagerGlobal.Instance.fase3SimbolosAcertos +
                " | Erros: " + GameManagerGlobal.Instance.fase3SimbolosErros + "\n\n" +
                "Marcador central:\n" +
                "Acertos: " + GameManagerGlobal.Instance.fase3CentroAcertos +
                " | Erros: " + GameManagerGlobal.Instance.fase3CentroErros + "\n\n" +
                "Quiz de programação:\n" +
                "Acertos: " + GameManagerGlobal.Instance.fase3QuizAcertos +
                " | Erros: " + GameManagerGlobal.Instance.fase3QuizErros;

            txtFase4.text =
                "FASE 4 - MÚLTIPLOS RECURSOS\n" +
                "Matemática:\n" +
                "Acertos: " + GameManagerGlobal.Instance.fase4MatematicaAcertos +
                " | Erros: " + GameManagerGlobal.Instance.fase4MatematicaErros + "\n\n" +
                "Estabilidade:\n" +
                "Acertos: " + GameManagerGlobal.Instance.fase4RastreamentoAcertos +
                " | Erros: " + GameManagerGlobal.Instance.fase4RastreamentoErros + "\n\n" +
                "Ritmo:\n" +
                "Acertos: " + GameManagerGlobal.Instance.fase4RitmoAcertos +
                " | Erros: " + GameManagerGlobal.Instance.fase4RitmoErros + "\n\n" +
                "Círculo:\n" +
                "Acertos: " + GameManagerGlobal.Instance.fase4CirculoAcertos +
                " | Erros: " + GameManagerGlobal.Instance.fase4CirculoErros;

            if (txtMensagemFinal != null)
                txtMensagemFinal.text = GerarMensagemFinal();
        }
        else
        {
            txtResumoGeral.text = "Nenhum resultado encontrado.";
        }
    }

    string GerarMensagemFinal()
    {
        int acertos = GameManagerGlobal.Instance.acertos;
        int erros = GameManagerGlobal.Instance.erros;

        if (acertos >= 50 && erros <= 15)
            return "Ótimo desempenho! Você conseguiu lidar bem com múltiplas tarefas.";

        if (acertos >= 30)
            return "Bom desempenho! Algumas tarefas causaram interferência cognitiva.";

        return "A sobrecarga cognitiva afetou bastante o desempenho.";
    }

    public void VoltarMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
