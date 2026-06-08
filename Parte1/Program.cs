// L1 = { w ∈ {a,b}* | w termina com "ab" }

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("════════════════════════════════════════════════════════");
Console.WriteLine("  PARTE 1 — Autômato Finito Determinístico (AFD)");
Console.WriteLine("  Linguagem L1 = { w ∈ {a,b}* | w termina com \"ab\" }");
Console.WriteLine("════════════════════════════════════════════════════════");
Console.WriteLine();

// Estados:
//   q0 — estado inicial: nenhuma parte relevante do sufixo foi lida ainda ou o último símbolo lido era 'b' 
//   q1 — "a" foi lido por último: potencial início do sufixo "ab"
//   q2 — "ab" foi lido como sufixo imediato: estado de aceitação

// Tabela de transições δ:
//   δ(q0, a) = q1   (pode estar iniciando "ab")
//   δ(q0, b) = q0   (não progredimos; voltamos a q0)
//   δ(q1, a) = q1   (o novo 'a' substitui o anterior)
//   δ(q1, b) = q2   (sufixo "ab" completo)
//   δ(q2, a) = q1   (novo potencial sufixo)
//   δ(q2, b) = q0   (sufixo "ab" quebrado)

// Expressão regular equivalente: (a|b)*ab

Console.WriteLine("── Construindo AFD para L1 (hardcoded) ──────────────────");
Console.WriteLine();

var Q = new HashSet<string> { "q0", "q1", "q2" };

var Sigma = new HashSet<char> { 'a', 'b' };

var Delta = new Dictionary<(string, char), string>
{
    // De q0
    { ("q0", 'a'), "q1" },   // leu 'a' → candidato ao início de "ab"
    { ("q0", 'b'), "q0" },   // leu 'b' → sem progresso

    // De q1  (acabou de ler 'a')
    { ("q1", 'a'), "q1" },   // outro 'a' → mantém candidatura
    { ("q1", 'b'), "q2" },   // leu 'b' → sufixo "ab" encontrado!

    // De q2  (sufixo "ab" lido; estado de aceitação)
    { ("q2", 'a'), "q1" },   // novo 'a' inicia outro candidato
    { ("q2", 'b'), "q0" },   // 'b' desfaz o sufixo
};


string estadoInicial = "q0";

var F = new HashSet<string> { "q2" };

var afdL1 = new AutomatoFinitoDeterministico(Q, Sigma, Delta, estadoInicial, F);

afdL1.ExibirDiagrama();

//Processar arquivo de entradas (entradas.txt)

string caminhoEntradas = "entradas.txt";

// Criar arquivo de entradas se não existir (casos de teste obrigatórios)
if (!File.Exists(caminhoEntradas))
{
    Console.WriteLine($"[INFO] Arquivo '{caminhoEntradas}' não encontrado. Criando com casos de teste padrão...");
    File.WriteAllLines(caminhoEntradas, new[]
    {
        "ab",       // ACEITA — caso mínimo
        "aab",      // REJEITA — não termina com ab
        "bab",      // ACEITA — prefixo irrelevante
        "ababab",   // ACEITA — múltiplas ocorrências
        "ba",       // REJEITA — termina com 'a' apenas
        "",         // REJEITA — cadeia vazia ε
        "b",        // REJEITA — um símbolo
    });
    Console.WriteLine($"[INFO] Arquivo '{caminhoEntradas}' criado com 7 casos de teste.");
    Console.WriteLine();
}

afdL1.ProcessarArquivo(caminhoEntradas);

// Carregar AFD a partir de arquivo JSON

Console.WriteLine("════════════════════════════════════════════════════════");
Console.WriteLine("  DESAFIO — Carregamento dinâmico de AFD via afd.json");
Console.WriteLine("════════════════════════════════════════════════════════");
Console.WriteLine();

string caminhoJson = "afd.json";

// Criar arquivo JSON padrão (mesmo AFD de L1) se não existir
if (!File.Exists(caminhoJson))
{
    Console.WriteLine($"[INFO] Arquivo '{caminhoJson}' não encontrado. Criando exemplo padrão (AFD para L1)...");
    string jsonPadrao = @"{
  ""estados"": [""q0"", ""q1"", ""q2""],
  ""alfabeto"": [""a"", ""b""],
  ""estadoInicial"": ""q0"",
  ""estadosAceitacao"": [""q2""],
  ""transicoes"": [
    { ""origem"": ""q0"", ""simbolo"": ""a"", ""destino"": ""q1"" },
    { ""origem"": ""q0"", ""simbolo"": ""b"", ""destino"": ""q0"" },
    { ""origem"": ""q1"", ""simbolo"": ""a"", ""destino"": ""q1"" },
    { ""origem"": ""q1"", ""simbolo"": ""b"", ""destino"": ""q2"" },
    { ""origem"": ""q2"", ""simbolo"": ""a"", ""destino"": ""q1"" },
    { ""origem"": ""q2"", ""simbolo"": ""b"", ""destino"": ""q0"" }
  ]
}";
    File.WriteAllText(caminhoJson, jsonPadrao);
    Console.WriteLine($"[INFO] Arquivo '{caminhoJson}' criado.");
    Console.WriteLine();
}

var afdJson = CarregadorAfd.Carregar(caminhoJson);

if (afdJson != null)
{
    Console.WriteLine("── Diagrama do AFD carregado via JSON ───────────────────");
    afdJson.ExibirDiagrama();

    Console.WriteLine("── Testando cadeias com o AFD carregado via JSON ────────");
    Console.WriteLine();

    var casosTeste = new[]
    {
        ("ab",      true,  "caso mínimo"),
        ("aab",     false, "não termina com ab"),
        ("bab",     true,  "prefixo irrelevante"),
        ("ababab",  true,  "múltiplas ocorrências"),
        ("ba",      false, "termina com 'a' apenas"),
        ("",        false, "cadeia vazia ε"),
        ("b",       false, "um símbolo"),
    };

    int corretos = 0;
    foreach (var (cadeia, esperado, justificativa) in casosTeste)
    {
        string exibicao = cadeia.Length == 0 ? "ε" : $"\"{cadeia}\"";
        Console.WriteLine($"  Cadeia {exibicao,-12} — {justificativa}");
        bool resultado = afdJson.Aceitar(cadeia);
        string status = resultado == esperado ? "✓ CORRETO" : "✗ INCORRETO";
        Console.WriteLine($"  Resultado: {(resultado ? "ACEITA" : "REJEITA")}  [{status}]");
        Console.WriteLine();
        if (resultado == esperado) corretos++;
    }

    Console.WriteLine($"  Acertos: {corretos}/{casosTeste.Length}");
}

Console.WriteLine();
Console.WriteLine("════════════════════════════════════════════════════════");
Console.WriteLine("  Execução concluída.");
Console.WriteLine("════════════════════════════════════════════════════════");
