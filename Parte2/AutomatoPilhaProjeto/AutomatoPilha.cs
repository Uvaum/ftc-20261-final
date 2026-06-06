using System;
using System.Collections.Generic;
using System.Linq;

public class AutomatoPilha
{
    private Stack<char> pilha; // Pilha do autômato
    public HashSet<string> Estado {get; private set;} // q conjunto finito de estados
    public HashSet<char> Entrada {get; private set;} // Σ alfabeto de entrada
    public HashSet<char> AlfabetoPilha {get; private set;} // Γ alfabeto da pilha
    private Dictionary<(string, char, char), (string, string)> transicoes; // δ função de transição
    public string EstadoInicial {get; private set;} // q0 estado inicial
    public char SimboloInicial {get; private set;} // Z0 simbolo inicial da pilha
    public HashSet<string> EstadoFinal {get; private set;} // F estados finais

    public AutomatoPilha()
    {
        pilha = new Stack<char>();
        Estado = new HashSet<string>{"q0", "q1"};
        Entrada = new HashSet<char>{'a', 'b'};
        AlfabetoPilha = new HashSet<char>{'a', 'Z'}; // 'a' para empilhar e 'Z' para simbolo inicial da pilha
        EstadoInicial = "q0";
        SimboloInicial = 'Z';
        transicoes = new Dictionary<(string, char, char), (string, string)>();
        EstadoFinal = new HashSet<string>(); // aceita por vazio
        TransicoesConfig(); // Popula o dicionário de transições
    }

    private void TransicoesConfig()
    {
        transicoes.Add(("q0", 'a', 'Z'), ("q0", "aZ")); // lê 'a' -> desempilha 'Z' -> q0 -> empilha 'a' e 'Z'

        transicoes.Add(("q0", 'a', 'a'), ("q0", "aa")); // lê 'a' -> desempilha 'a' -> q0 -> empilha 'a' e 'a'

        transicoes.Add(("q0", 'b', 'a'), ("q1", "")); // lê 'b' -> desempilha 'a' -> q1 -> não empilha nada

        transicoes.Add(("q1", 'b', 'a'), ("q1", "")); // lê 'b' -> desempilha 'a' -> q1 -> não empilha nada

        transicoes.Add(("q1", '\0', 'Z'), ("q1", "")); // lê \0' (vazio) -> desempilha 'Z' -> q1 -> não empilha nada
    }

    public bool Simular(String entrada)
    {

        pilha.Clear();
        pilha.Push(SimboloInicial); // Empilha o símbolo inicial da pilha
        string estadoAtual = EstadoInicial;

        Console.WriteLine($"\nSimulando: {entrada}");
        ExibirConfig(estadoAtual, entrada);

        Queue<char> filaEntrada = new Queue<char>(entrada);

        while (filaEntrada.Count > 0)
        {
            char simboloEntrada = filaEntrada.Dequeue();
            
            if(pilha.Count == 0)
            {
                return false; // Se a pilha estiver vazia, a entrada não é aceita
            }

            char topo = pilha.Pop(); // Desempilha o topo da pilha

            if(transicoes.TryGetValue((estadoAtual, simboloEntrada, topo), out var transicao))
            {
                estadoAtual = transicao.Item1;
                string empilhar = transicao.Item2;

                for(int i = empilhar.Length - 1; i >= 0; i--)
                {
                    pilha.Push(empilhar[i]); // Empilha os símbolos na ordem correta
                }
                ExibirConfig(estadoAtual, new string(filaEntrada.ToArray()));
            }
            else
            {
                pilha.Push(topo);
                break; // Se não houver transição válida, interrompe a simulação
            }
           
        }

        if(pilha.Count > 0)
        {
            // Verifica se é possível aceitar a entrada por vazio
            char topo = pilha.Pop();
            if(transicoes.TryGetValue((estadoAtual, '\0', topo), out var transicao))
            {
                estadoAtual = transicao.Item1;
                ExibirConfig(estadoAtual, "\0");
            }
            else
            {
                pilha.Push(topo); // Restaura o topo da pilha se não houver transição por vazio
            }
        }
        
        bool aceita = pilha.Count == 0; // Aceita se a pilha estiver vazia no final da simulação
        Console.WriteLine(aceita ? "Entrada aceita!" : "Entrada rejeitada!");
        return aceita;
    }

    private void ExibirConfig(string estado, string entradaRestante){

        string conteudo = pilha.Count > 0 ? string.Join("", pilha.ToArray()) : "vazia";
        string entradaFormat = string.IsNullOrEmpty(entradaRestante) ? "E" : entradaRestante;

        Console.WriteLine($"Estado: {estado}, Entrada: {entradaFormat}, Conteúdo da Pilha: {conteudo}");
    }
}