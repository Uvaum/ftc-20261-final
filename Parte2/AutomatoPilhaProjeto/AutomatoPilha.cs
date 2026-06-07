using System;
using System.Collections.Generic;
using System.Linq;

public class AutomatoPilha
{
    private Stack<char> pilha; // Pilha do autômato
    public HashSet<string> Estado {get; private set;} // q conjunto finito de estados
    public HashSet<char> Entrada {get; private set;} // Σ alfabeto de entrada
    public HashSet<char> AlfabetoPilha {get; private set;} // Γ alfabeto da pilha
    private Dictionary<(string, char, char), List<(string, string)>> transicoes; // δ função de transição
    public string EstadoInicial {get; private set;} // q0 estado inicial
    public char SimboloInicial {get; private set;} // Z0 simbolo inicial da pilha
    public HashSet<string> EstadoFinal {get; private set;} // F estados finais

    public AutomatoPilha()
    {
        pilha = new Stack<char>();
        transicoes = new Dictionary<(string, char, char), List<(string, string)>>();
        Estado = new HashSet<string>();
        Entrada = new HashSet<char>();
        AlfabetoPilha = new HashSet<char>();
        EstadoFinal = new HashSet<string>();
        EstadoInicial = "q0";
        SimboloInicial = 'Z';
    }

    public void L2Config()
    {
        transicoes.Clear();
        EstadoInicial = "q0";
        SimboloInicial = 'Z';
        Entrada = new HashSet<char>{'a', 'b'};

        AdicionarTransicao("q0", 'a', 'Z', "q0", "aZ"); // Empilha 'a' sobre 'Z'
        AdicionarTransicao("q0", 'a', 'a', "q0", "aa"); // Empilha 'a' sobre 'a'
        AdicionarTransicao("q0", 'b', 'a', "q1", ""); // Desempilha 'a' ao ler 'b'
        AdicionarTransicao("q1", 'b', 'a', "q1", ""); // Continua desempilhando 'a' ao ler 'b'
        AdicionarTransicao("q1", '\0', 'Z', "q1", ""); // Aceita por vazio quando a pilha estiver vazia

    }

    public void L3Config()
    {
        transicoes.Clear();
        EstadoInicial = "q0";
        SimboloInicial = 'Z';
        Entrada = new HashSet<char>{'a', 'b'};
        char[] alfabetoPilha = {'a', 'b', 'Z'}; // 'a' e 'b' para empilhar, 'Z' para simbolo inicial da pilha

        foreach(char topo in alfabetoPilha)
        {
            AdicionarTransicao("q0", 'a', topo, "q0", "a" + topo); // Empilha 'a' sobre o topo atual
            AdicionarTransicao("q0", 'b', topo, "q0", "b" + topo); // Empilha 'b' sobre o topo atual
            AdicionarTransicao("q0", 'a', topo, "q1", topo.ToString()); // Transição para q1 sem empilhar nada (desempilha o topo)
            AdicionarTransicao("q0", 'b', topo, "q1", topo.ToString()); // Transição para q1 sem empilhar nada (desempilha o topo)
            AdicionarTransicao("q0", '\0', topo, "q1", topo.ToString()); // Transição para q1 por vazio (desempilha o topo)

        }

        AdicionarTransicao("q1", 'a', 'a', "q1", ""); // Continua desempilhando 'a' ao ler 'a'
        AdicionarTransicao("q1", 'b', 'b', "q1", ""); // Continua desempilhando 'b' ao ler 'b'
        AdicionarTransicao("q1", '\0', 'Z', "q1", ""); // Aceita por vazio quando a pilha estiver vazia

    }

    public void AdicionarTransicao(string estadoAtual, char entrada, char topo, string novoEstado, string empilhar)
    {
        
        var chave = (estadoAtual, entrada, topo);
        if (!transicoes.ContainsKey(chave))
        {
            transicoes[chave] = new List<(string, string)>();
        }
        transicoes[chave].Add((novoEstado, empilhar));
    }

    public bool Simular(String entrada)
    {
        if(String.IsNullOrEmpty(entrada))
        {
            Console.WriteLine("ERRO: Entrada vazia.");
            return false;
        }

        foreach(char simbolo in entrada)
        {
            if(!Entrada.Contains(simbolo))
            {
                Console.WriteLine($"ERRO: entrada contém símbolo inválido.");
                return false; // Se a entrada contiver um símbolo inválido, a entrada não é aceita
            }
        }

        Console.WriteLine($"\nSimulando: {entrada}");

        Stack<char> pilhaInicial = new Stack<char>();
        pilhaInicial.Push(SimboloInicial); // Inicia a pilha com o símbolo inicial

        List<string> historico = new List<string>
        {
            $"Estado Inicial: {EstadoInicial}, Entrada: {entrada}, Pilha: {SimboloInicial}"
        };
        
        bool aceita = Explorar(EstadoInicial, entrada, 0, pilhaInicial, historico);
        Console.WriteLine(aceita ? "Entrada aceita!" : "Entrada rejeitada!");
        return aceita;
    }

    private bool Explorar(string estadoAtual, string cadeia, int indiceEntrada, Stack<char> pilhaAtual, List<string> historico)
    {
        // Condição de Aceitação: leu toda a palavra e a pilha esvaziou
        if(indiceEntrada == cadeia.Length && pilhaAtual.Count == 0)
        {
            foreach(var passo in historico)
            {
                Console.WriteLine(passo);
            }
            return true; 
        }

        if(pilhaAtual.Count == 0) // Se a pilha esvaziou mas ainda há entrada para ler, essa configuração não é válida
        {
            return false; 
        }

        char topoPilha = pilhaAtual.Pop(); 
        string entradaRestante = indiceEntrada < cadeia.Length ? cadeia.Substring(indiceEntrada) : "\0";

        if(indiceEntrada < cadeia.Length)
        {
            char simboloEntrada = cadeia[indiceEntrada];
            if(transicoes.TryGetValue((estadoAtual, simboloEntrada, topoPilha), out var transicoesPossiveis)) // Verifica se há transições possíveis para o símbolo de entrada atual, o estado atual e o topo da pilha
            {
                foreach(var (novoEstado, empilhar) in transicoesPossiveis)
                {
                    Stack<char> novaPilha = ClonarPilha(pilhaAtual);
                    EmpilharMultiplos(novaPilha, empilhar);

                    List<string> novoHistorico = new List<string>(historico);
                    novoHistorico.Add(GerarStringConfig(novoEstado, cadeia.Substring(indiceEntrada + 1), novaPilha));

                    // Chamada recursiva: avança o índice da entrada (+1)
                    if (Explorar(novoEstado, cadeia, indiceEntrada + 1, novaPilha, novoHistorico)) return true;
                }
            }
        }

        if(transicoes.TryGetValue((estadoAtual, '\0', topoPilha), out var transicoesLambda)) // Verifica se há transições lambda (vazio) para o estado atual e o topo da pilha
        {
            foreach(var (novoEstado, empilhar) in transicoesLambda)
            {
                Stack<char> novaPilha = ClonarPilha(pilhaAtual);
                EmpilharMultiplos(novaPilha, empilhar);

                List<string> novoHistorico = new List<string>(historico);
                novoHistorico.Add(GerarStringConfig(novoEstado, entradaRestante, novaPilha));

                // Chamada recursiva: O índice de entrada NÃO avança
                if (Explorar(novoEstado, cadeia, indiceEntrada, novaPilha, novoHistorico)) return true;
            }
        }

        // Se explorou todas as opções deste nó e nenhuma deu certo, retorna falso (Backtracking)
        return false;
    }

    private void EmpilharMultiplos(Stack<char> p, string simbolos)
    {
        for (int i = simbolos.Length - 1; i >= 0; i--) p.Push(simbolos[i]);
    }

    private Stack<char> ClonarPilha(Stack<char> original)
    {
        var array = original.ToArray();
        Array.Reverse(array); // Inverter a array garante que a nova Stack mantenha a ordem do topo original
        return new Stack<char>(array);
    }

    private string GerarStringConfig(string estado, string entradaRestante, Stack<char> p)
    {
        string conteudo = p.Count > 0 ? string.Join("", p.ToArray()) : "[Vazia]";
        string entradaFormat = string.IsNullOrEmpty(entradaRestante) || entradaRestante == "\0" ? "λ" : entradaRestante;
        return $"Estado: {estado}, Entrada: {entradaFormat}, Conteúdo da Pilha: {conteudo}";
    }
}