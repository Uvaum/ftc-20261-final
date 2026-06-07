
// Implementação formal do AFD como a 5-tupla M = (Q, Σ, δ, q0, F)
//   Q  = conjunto finito de estados
//   Σ  = alfabeto finito de entrada
//   δ  = função de transição  Q × Σ → Q
//   q0 = estado inicial  (q0 ∈ Q)
//   F  = conjunto de estados de aceitação  (F ⊆ Q)

public class AutomatoFinitoDeterministico
{
   
    public HashSet<string> Q { get; private set; }
    public HashSet<char> Sigma { get; private set; }
    public Dictionary<(string estado, char simbolo), string> Delta { get; private set; }
    public string q0 { get; private set; }
    public HashSet<string> F { get; private set; }


// Construtor: valida se q0 pertence a Q e se F é subconjunto de Q
    public AutomatoFinitoDeterministico(
        HashSet<string> Q,
        HashSet<char> Sigma,
        Dictionary<(string, char), string> Delta,
        string q0,
        HashSet<string> F)
    {
        // Validação formal: q0 deve pertencer a Q
        if (!Q.Contains(q0))
            throw new ArgumentException($"Estado inicial '{q0}' não pertence ao conjunto Q.");

        // Validação formal: F deve ser subconjunto de Q
        foreach (var estado in F)
        {
            if (!Q.Contains(estado))
                throw new ArgumentException($"Estado de aceitação '{estado}' não pertence ao conjunto Q.");
        }

        this.Q = Q;
        this.Sigma = Sigma;
        this.Delta = Delta;
        this.q0 = q0;
        this.F = F;
    }

    // Método principal: simulação da leitura de uma cadeia

    public bool Aceitar(string cadeia)
    {
        string estadoAtual = q0;          // começa no estado inicial q0
        var rastro = new List<string> { estadoAtual };

        foreach (char simbolo in cadeia)
        {
            // Verificar se o símbolo pertence ao alfabeto Σ
            if (!Sigma.Contains(simbolo))
            {
                Console.WriteLine($"  [ERRO] Símbolo '{simbolo}' não pertence ao alfabeto Σ = {{{string.Join(", ", Sigma)}}}.");
                Console.WriteLine($"  Rastro: {string.Join(" → ", rastro)} → [rejeição por símbolo inválido]");
                return false;
            }

            // Aplicar δ(estadoAtual, símbolo)
            var chave = (estadoAtual, simbolo);
            if (!Delta.TryGetValue(chave, out string? proximoEstado))
            {
                // δ não está definida para este par — transição para estado "morto" implícito
                Console.WriteLine($"  Rastro: {string.Join(" → ", rastro)} → [estado morto]");
                return false;
            }

            estadoAtual = proximoEstado;
            rastro.Add(estadoAtual);
        }

        // Exibir rastro completo
        Console.WriteLine($"  Rastro: {string.Join(" → ", rastro)}");

        // Aceita se o estado final pertence a F
        return F.Contains(estadoAtual);
    }

// Método: exibir tabela de transições 
// Imprime no console a representação textual da tabela de transições δ
// Linhas = estados (Q); Colunas = símbolos (Σ)
// Marcações: "→" indica q0, "*" indica estados em F

    public void ExibirDiagrama()
    {
        int larguraEstado = Math.Max(8, Q.Max(e => e.Length) + 4);
        int larguraSimbolo = 6;

        Console.WriteLine();
        Console.WriteLine("╔══ TABELA DE TRANSIÇÕES — δ : Q × Σ → Q ══╗");
        Console.WriteLine();

        // Cabeçalho: prefixo + colunas de símbolos
        string cabecalho = "  " + "Estado".PadRight(larguraEstado);
        foreach (char s in Sigma.OrderBy(c => c))
            cabecalho += $"  δ(·,{s})".PadRight(larguraSimbolo + 2);
        Console.WriteLine(cabecalho);
        Console.WriteLine("  " + new string('─', larguraEstado + Sigma.Count * (larguraSimbolo + 2)));

        // Linhas: uma por estado em Q
        foreach (string estado in Q.OrderBy(e => e))
        {
            // Marcações: "→" para estado inicial, "*" para estados de aceitação
            string prefixo = "";
            if (estado == q0 && F.Contains(estado)) prefixo = "→* ";
            else if (estado == q0)                  prefixo = "→  ";
            else if (F.Contains(estado))            prefixo = "*  ";
            else                                    prefixo = "   ";

            string linha = prefixo + estado.PadRight(larguraEstado - 1);

            foreach (char s in Sigma.OrderBy(c => c))
            {
                var chave = (estado, s);
                string destino = Delta.TryGetValue(chave, out string? dest) ? dest : "—";
                linha += ("  " + destino).PadRight(larguraSimbolo + 2);
            }
            Console.WriteLine(linha);
        }

        Console.WriteLine();
        Console.WriteLine("  Legenda:  → estado inicial   * estado de aceitação   — transição não definida");
        Console.WriteLine("╚════════════════════════════════════════════╝");
        Console.WriteLine();
    }


// Método: processar arquivo de entradas
// Lê cadeias de um arquivo de texto (uma por linha) e exibe para cada cadeia: o valor, o rastro de estados e o resultado. Linhas em branco são interpretadas como a cadeia vazia ε

    public void ProcessarArquivo(string caminhoArquivo)
    {
        if (!File.Exists(caminhoArquivo))
        {
            Console.WriteLine($"[ERRO] Arquivo '{caminhoArquivo}' não encontrado.");
            return;
        }

        string[] linhas = File.ReadAllLines(caminhoArquivo);
        Console.WriteLine($"╔══ PROCESSANDO ARQUIVO: {caminhoArquivo} ══╗");
        Console.WriteLine($"  Total de cadeias: {linhas.Length}");
        Console.WriteLine();

        int aceitas = 0, rejeitadas = 0;

        for (int i = 0; i < linhas.Length; i++)
        {
            // Linha em branco = cadeia vazia ε
            string cadeia = linhas[i];
            string exibicao = cadeia.Length == 0 ? "ε (vazia)" : $"\"{cadeia}\"";

            Console.WriteLine($"[{i + 1:D2}] Cadeia: {exibicao}");

            bool resultado = Aceitar(cadeia);
            string decisao = resultado ? "✓ ACEITA" : "✗ REJEITA";
            Console.WriteLine($"  Resultado: {decisao}");

            if (resultado) aceitas++;
            else rejeitadas++;

            Console.WriteLine();
        }

        // Resumo
        Console.WriteLine($"╔══ RESUMO ══╗");
        Console.WriteLine($"  Aceitas:   {aceitas}");
        Console.WriteLine($"  Rejeitadas: {rejeitadas}");
        Console.WriteLine($"  Total:      {linhas.Length}");
        Console.WriteLine();
    }
}
