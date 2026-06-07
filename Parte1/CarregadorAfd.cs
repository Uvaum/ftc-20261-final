// Carrega qualquer AFD a partir de um arquivo de configuração afd.json

using System.Text.Json;

public class ConfiguracaoAfd
{
    public List<string> estados { get; set; } = new();
    public List<string> alfabeto { get; set; } = new();
    public string estadoInicial { get; set; } = "";
    public List<string> estadosAceitacao { get; set; } = new();
    public List<Transicao> transicoes { get; set; } = new();
}

// Transição δ(origem, símbolo) = destino no JSON
public class Transicao
{
    public string origem { get; set; } = "";
    public string simbolo { get; set; } = "";
    public string destino { get; set; } = "";
}


// Carrega e valida um AFD a partir de um arquivo JSON

public static class CarregadorAfd
{
    public static AutomatoFinitoDeterministico? Carregar(string caminhoJson)
    {
        if (!File.Exists(caminhoJson))
        {
            Console.WriteLine($"[ERRO] Arquivo de configuração '{caminhoJson}' não encontrado.");
            return null;
        }

        ConfiguracaoAfd config;
        try
        {
            string json = File.ReadAllText(caminhoJson);
            var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            config = JsonSerializer.Deserialize<ConfiguracaoAfd>(json, opcoes)
                     ?? throw new InvalidOperationException("JSON nulo após desserialização.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERRO] Falha ao ler JSON: {ex.Message}");
            return null;
        }

        // ── Validação dos campos obrigatórios 
        var erros = new List<string>();

        if (config.estados.Count == 0)
            erros.Add("Campo 'estados' está vazio.");

        if (config.alfabeto.Count == 0)
            erros.Add("Campo 'alfabeto' está vazio.");

        if (string.IsNullOrWhiteSpace(config.estadoInicial))
            erros.Add("Campo 'estadoInicial' está vazio.");

        // Validar que cada símbolo do alfabeto tem exatamente 1 caractere
        foreach (var s in config.alfabeto)
        {
            if (s.Length != 1)
                erros.Add($"Símbolo de alfabeto inválido: '{s}' (deve ter exatamente 1 caractere).");
        }

        // Validar que o símbolo em cada transição tem exatamente 1 caractere
        foreach (var t in config.transicoes)
        {
            if (t.simbolo.Length != 1)
                erros.Add($"Símbolo inválido na transição ({t.origem}, '{t.simbolo}') → '{t.destino}'.");

            if (!config.estados.Contains(t.origem))
                erros.Add($"Estado origem '{t.origem}' na transição não pertence a Q.");

            if (!config.estados.Contains(t.destino))
                erros.Add($"Estado destino '{t.destino}' na transição não pertence a Q.");

            if (!config.alfabeto.Contains(t.simbolo))
                erros.Add($"Símbolo '{t.simbolo}' na transição não pertence a Σ.");
        }

        // Exibir erros de validação 
        if (erros.Count > 0)
        {
            Console.WriteLine("[ERRO] Configuração inválida:");
            foreach (var e in erros)
                Console.WriteLine($"  • {e}");
            return null;
        }

        // ── Construção da 5-tupla 
        var Q = new HashSet<string>(config.estados);
        var Sigma = new HashSet<char>(config.alfabeto.Select(s => s[0]));

        var Delta = new Dictionary<(string, char), string>();
        foreach (var t in config.transicoes)
        {
            var chave = (t.origem, t.simbolo[0]);
            if (Delta.ContainsKey(chave))
            {
                Console.WriteLine($"[AVISO] Transição duplicada para ({t.origem}, '{t.simbolo}'). " +
                                  $"O AFD não é determinístico — usando a última definição.");
            }
            Delta[chave] = t.destino;
        }

        var F = new HashSet<string>(config.estadosAceitacao);

        Console.WriteLine($"[OK] AFD carregado de '{caminhoJson}'.");
        Console.WriteLine($"     |Q| = {Q.Count}  |Σ| = {Sigma.Count}  " +
                          $"|δ| = {Delta.Count} transições  " +
                          $"|F| = {F.Count} estado(s) de aceitação");
        Console.WriteLine();

        return new AutomatoFinitoDeterministico(Q, Sigma, Delta, config.estadoInicial, F);
    }
}
