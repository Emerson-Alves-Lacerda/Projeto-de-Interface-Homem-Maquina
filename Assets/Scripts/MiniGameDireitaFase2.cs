using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MiniGameDireitaFase2 : MonoBehaviour
{
    [System.Serializable]
    public class Pergunta
    {
        public string pergunta;
        public string opcao1;
        public string opcao2;
        public string opcao3;
        public int correta; // 1, 2 ou 3
    }

    public Text txtInstrucao;
    public Button btnOpcao1;
    public Button btnOpcao2;
    public Button btnOpcao3;

    private List<Pergunta> perguntas = new List<Pergunta>();
    private Pergunta perguntaAtual;

    void Start()
    {
        MontarPerguntas();
        GerarDesafio();
    }

    void MontarPerguntas()
    {
        perguntas.Add(new Pergunta
        {
            pergunta = "Em Python, qual palavra-chave cria uma função?",
            opcao1 = "func",
            opcao2 = "def",
            opcao3 = "method",
            correta = 2
        });

        perguntas.Add(new Pergunta
        {
            pergunta = "Em Java, qual tipo armazena texto?",
            opcao1 = "String",
            opcao2 = "char",
            opcao3 = "bool",
            correta = 1
        });

        perguntas.Add(new Pergunta
        {
            pergunta = "Em C++, qual biblioteca é usada para entrada e saída básica?",
            opcao1 = "math",
            opcao2 = "scanner",
            opcao3 = "iostream",
            correta = 3
        });

        perguntas.Add(new Pergunta
        {
            pergunta = "Em Python, qual estrutura repete enquanto a condição for verdadeira?",
            opcao1 = "repeat",
            opcao2 = "while",
            opcao3 = "loop",
            correta = 2
        });

        perguntas.Add(new Pergunta
        {
            pergunta = "Em Java, qual palavra define uma classe pública?",
            opcao1 = "public class",
            opcao2 = "new class",
            opcao3 = "main class",
            correta = 1
        });

        perguntas.Add(new Pergunta
        {
            pergunta = "Em C++, qual operador é usado com cout para imprimir?",
            opcao1 = "=>",
            opcao2 = "::",
            opcao3 = "<<",
            correta = 3
        });
    }

    void GerarDesafio()
    {
        int index = Random.Range(0, perguntas.Count);
        perguntaAtual = perguntas[index];

        if (txtInstrucao != null)
            txtInstrucao.text = perguntaAtual.pergunta;

        if (btnOpcao1 != null)
            btnOpcao1.GetComponentInChildren<Text>().text = perguntaAtual.opcao1;

        if (btnOpcao2 != null)
            btnOpcao2.GetComponentInChildren<Text>().text = perguntaAtual.opcao2;

        if (btnOpcao3 != null)
            btnOpcao3.GetComponentInChildren<Text>().text = perguntaAtual.opcao3;
    }

    void VerificarOpcao(int opcaoSelecionada)
    {
        if (opcaoSelecionada == perguntaAtual.correta)
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarAcerto();
        }
        else
        {
            if (GameManagerGlobal.Instance != null)
                GameManagerGlobal.Instance.RegistrarErro();
        }

        GerarDesafio();
    }

    public void ClicarOpcao1()
    {
        VerificarOpcao(1);
    }

    public void ClicarOpcao2()
    {
        VerificarOpcao(2);
    }

    public void ClicarOpcao3()
    {
        VerificarOpcao(3);
    }
}
