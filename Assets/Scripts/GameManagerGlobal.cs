using UnityEngine;

public class GameManagerGlobal : MonoBehaviour
{
    public static GameManagerGlobal Instance;

    public int acertos = 0;
    public int erros = 0;
    public int nivel = 1;

    // Fase 1
    public int acertosBaixaCarga = 0;
    public int errosBaixaCarga = 0;
    public float tempoMedioBaixaCarga = 0f;

    public int acertosAltaCarga = 0;
    public int errosAltaCarga = 0;
    public float tempoMedioAltaCarga = 0f;

    // Fase 2
    public int fase2MemoriaAcertos = 0;
    public int fase2MemoriaErros = 0;
    public int fase2QuizAcertos = 0;
    public int fase2QuizErros = 0;

    // Fase 3
    public int fase3SimbolosAcertos = 0;
    public int fase3SimbolosErros = 0;
    public int fase3CentroAcertos = 0;
    public int fase3CentroErros = 0;
    public int fase3QuizAcertos = 0;
    public int fase3QuizErros = 0;

    // Fase 4
    public int fase4MatematicaAcertos = 0;
    public int fase4MatematicaErros = 0;
    public int fase4RastreamentoAcertos = 0;
    public int fase4RastreamentoErros = 0;
    public int fase4RitmoAcertos = 0;
    public int fase4RitmoErros = 0;
    public int fase4CirculoAcertos = 0;
    public int fase4CirculoErros = 0;

    private float tempoInicio;
    private float tempoFim;
    private bool jogoRodando = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IniciarJogo()
    {
        acertos = 0;
        erros = 0;
        nivel = 1;

        acertosBaixaCarga = 0;
        errosBaixaCarga = 0;
        tempoMedioBaixaCarga = 0f;

        acertosAltaCarga = 0;
        errosAltaCarga = 0;
        tempoMedioAltaCarga = 0f;

        fase2MemoriaAcertos = 0;
        fase2MemoriaErros = 0;
        fase2QuizAcertos = 0;
        fase2QuizErros = 0;

        fase3SimbolosAcertos = 0;
        fase3SimbolosErros = 0;
        fase3CentroAcertos = 0;
        fase3CentroErros = 0;
        fase3QuizAcertos = 0;
        fase3QuizErros = 0;

        fase4MatematicaAcertos = 0;
        fase4MatematicaErros = 0;
        fase4RastreamentoAcertos = 0;
        fase4RastreamentoErros = 0;
        fase4RitmoAcertos = 0;
        fase4RitmoErros = 0;
        fase4CirculoAcertos = 0;
        fase4CirculoErros = 0;

        tempoInicio = Time.time;
        tempoFim = 0f;
        jogoRodando = true;
    }

    public void RegistrarAcerto()
    {
        acertos++;
    }

    public void RegistrarErro()
    {
        erros++;
    }

    public void RegistrarFase2MemoriaAcerto() { fase2MemoriaAcertos++; RegistrarAcerto(); }
    public void RegistrarFase2MemoriaErro() { fase2MemoriaErros++; RegistrarErro(); }

    public void RegistrarFase2QuizAcerto() { fase2QuizAcertos++; RegistrarAcerto(); }
    public void RegistrarFase2QuizErro() { fase2QuizErros++; RegistrarErro(); }

    public void RegistrarFase3SimbolosAcerto() { fase3SimbolosAcertos++; RegistrarAcerto(); }
    public void RegistrarFase3SimbolosErro() { fase3SimbolosErros++; RegistrarErro(); }

    public void RegistrarFase3CentroAcerto() { fase3CentroAcertos++; RegistrarAcerto(); }
    public void RegistrarFase3CentroErro() { fase3CentroErros++; RegistrarErro(); }

    public void RegistrarFase3QuizAcerto() { fase3QuizAcertos++; RegistrarAcerto(); }
    public void RegistrarFase3QuizErro() { fase3QuizErros++; RegistrarErro(); }

    public void RegistrarFase4MatematicaAcerto() { fase4MatematicaAcertos++; RegistrarAcerto(); }
    public void RegistrarFase4MatematicaErro() { fase4MatematicaErros++; RegistrarErro(); }

    public void RegistrarFase4RastreamentoAcerto() { fase4RastreamentoAcertos++; RegistrarAcerto(); }
    public void RegistrarFase4RastreamentoErro() { fase4RastreamentoErros++; RegistrarErro(); }

    public void RegistrarFase4RitmoAcerto() { fase4RitmoAcertos++; RegistrarAcerto(); }
    public void RegistrarFase4RitmoErro() { fase4RitmoErros++; RegistrarErro(); }

    public void RegistrarFase4CirculoAcerto() { fase4CirculoAcertos++; RegistrarAcerto(); }
    public void RegistrarFase4CirculoErro() { fase4CirculoErros++; RegistrarErro(); }

    public void DefinirNivel(int novoNivel)
    {
        nivel = novoNivel;
    }

    public void FinalizarJogo()
    {
        if (jogoRodando)
        {
            tempoFim = Time.time;
            jogoRodando = false;
        }
    }

    public float GetTempoDecorrido()
    {
        if (jogoRodando)
        {
            return Time.time - tempoInicio;
        }

        return tempoFim - tempoInicio;
    }
}