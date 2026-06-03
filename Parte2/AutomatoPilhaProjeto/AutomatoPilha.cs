using System;
using System.Collections.Generic;
using System.Linq;

public class AutomatoPilha
{
    private Stack<char> pilha; // Pilha do autômato
    public HashSet<string> Estado {get; private set;} // q conjunto finito de estados
    public HashSet<char> Entrada {get; private set;} // Σ alfabeto de entrada
    public HashSet<char> AlfabetoPilha {get; private set;} // Γ alfabeto da pilha
    private Dictionary<(string, char, char), (string, string)> Transicoes; // δ função de transição
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
        Transicoes = new Dictionary<(string, char, char), (string, string)>();
        EstadoFinal = new HashSet<string>(); // aceita por vazio

    }

}