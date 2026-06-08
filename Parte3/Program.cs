using Parte3;

PrintHeader();
RunL4Tests();
RunL4Interactive();
RunIncrementerTests();
RunIncrementerInteractive();

static void PrintHeader()
{
    Console.WriteLine("=======================================================");
    Console.WriteLine("  Fundamentos Teoricos da Computacao -- Parte 3");
    Console.WriteLine("  Implementacao de Maquinas de Turing em C#");
    Console.WriteLine("=======================================================");
    Console.WriteLine();
}

// ── MT 1: L4 ────────────────────────────────────────────────────────────────

static void RunL4Tests()
{
    Console.WriteLine("--- MT 1: Reconhecedor de L4 = { a^n b^n c^n | n >= 1 } ---");
    Console.WriteLine();

    var mt = BuildL4Recognizer();
    var inputs = File.ReadAllLines("entradas_l4.txt");

    var first = inputs[0];
    Console.WriteLine($"Trace passo a passo para \"{first}\":");
    mt.Run(first, (step, state, tape, head) =>
        Console.WriteLine($"  Passo {step,2} | estado: {state,-9} | cab: {head,2} | fita: {tape}"));
    Console.WriteLine();

    Console.WriteLine("Resultados:");
    foreach (var line in inputs)
        PrintL4Result(mt, line);

    Console.WriteLine();
}

static void RunL4Interactive()
{
    var mt = BuildL4Recognizer();

    Console.WriteLine("--- MT 1: Modo interativo (Enter para sair) ---");
    Console.WriteLine("  Alfabeto de entrada: { a, b, c }");
    Console.WriteLine();

    while (true)
    {
        Console.Write("  Cadeia: ");
        var input = Console.ReadLine() ?? "";

        if (input.Length == 0)
            break;

        if (!input.All(c => c is 'a' or 'b' or 'c'))
        {
            Console.WriteLine("  Entrada invalida: use apenas os simbolos { a, b, c }");
            Console.WriteLine();
            continue;
        }

        PrintL4Result(mt, input);
        Console.WriteLine();
    }

    Console.WriteLine();
}

static void PrintL4Result(TuringMachine mt, string input)
{
    var display = input.Length == 0 ? "(vazio)" : input;
    try
    {
        var result = mt.Run(input);
        var verdict = result.Accepted ? "ACEITA" : "REJEITA";
        Console.WriteLine($"  \"{display,-12}\" => {verdict,-8} ({result.StepsExecuted} passos)");
    }
    catch (StepLimitExceededException ex)
    {
        Console.WriteLine($"  \"{display,-12}\" => ERRO: {ex.Message}");
    }
}

static TuringMachine BuildL4Recognizer()
{
    var t = new Dictionary<(string, char), (string, char, char)>
    {
        // q0: procura proximo 'a' nao marcado
        { ("q0", 'X'), ("q0", 'X', 'R') },
        { ("q0", 'a'), ("q1", 'X', 'R') },
        { ("q0", 'Y'), ("q4", 'Y', 'R') },

        // q1: avanca ate o proximo 'b' nao marcado
        { ("q1", 'a'), ("q1", 'a', 'R') },
        { ("q1", 'Y'), ("q1", 'Y', 'R') },
        { ("q1", 'b'), ("q2", 'Y', 'R') },

        // q2: avanca ate o proximo 'c' nao marcado
        { ("q2", 'b'), ("q2", 'b', 'R') },
        { ("q2", 'Z'), ("q2", 'Z', 'R') },
        { ("q2", 'c'), ("q3", 'Z', 'L') },

        // q3: retrocede ao inicio para a proxima rodada
        { ("q3", 'X'), ("q3", 'X', 'L') },
        { ("q3", 'Y'), ("q3", 'Y', 'L') },
        { ("q3", 'Z'), ("q3", 'Z', 'L') },
        { ("q3", 'a'), ("q3", 'a', 'L') },
        { ("q3", 'b'), ("q3", 'b', 'L') },
        { ("q3", '_'), ("q0", '_', 'R') },

        // q4: verifica que todos os b's e c's foram marcados
        { ("q4", 'Y'), ("q4", 'Y', 'R') },
        { ("q4", 'Z'), ("q4", 'Z', 'R') },
        { ("q4", '_'), ("qaccept", '_', 'R') },
    };

    return new TuringMachine(t, "q0", "qaccept", "qreject");
}

// ── MT 2: Incrementador ──────────────────────────────────────────────────────

static void RunIncrementerTests()
{
    Console.WriteLine("--- MT 2: Incrementador Unario f(n) = n + 1 ---");
    Console.WriteLine();

    var mt = BuildUnaryIncrementer();
    var inputs = File.ReadAllLines("entradas_inc.txt");

    var first = inputs[0];
    Console.WriteLine($"Trace passo a passo para \"{first}\":");
    mt.Run(first, (step, state, tape, head) =>
        Console.WriteLine($"  Passo {step,2} | estado: {state,-9} | cab: {head,2} | fita: {tape}"));
    Console.WriteLine();

    Console.WriteLine("Resultados:");
    foreach (var line in inputs)
        PrintIncResult(mt, line);

    Console.WriteLine();
}

static void RunIncrementerInteractive()
{
    var mt = BuildUnaryIncrementer();

    Console.WriteLine("--- MT 2: Modo interativo (Enter para sair) ---");
    Console.WriteLine("  Alfabeto de entrada: { 1 }");
    Console.WriteLine();

    while (true)
    {
        Console.Write("  Cadeia: ");
        var input = Console.ReadLine() ?? "";

        if (input.Length == 0)
            break;

        if (!input.All(c => c == '1'))
        {
            Console.WriteLine("  Entrada invalida: use apenas o simbolo { 1 }");
            Console.WriteLine();
            continue;
        }

        PrintIncResult(mt, input);
        Console.WriteLine();
    }

    Console.WriteLine();
}

static void PrintIncResult(TuringMachine mt, string input)
{
    var display = input.Length == 0 ? "(vazio)" : input;
    try
    {
        var result = mt.Run(input);
        var ones = result.FinalTapeContent.Count(c => c == '1');
        Console.WriteLine($"  f({input.Length}) = {ones}  fita: [{result.FinalTapeContent}]  ({result.StepsExecuted} passos)");
    }
    catch (StepLimitExceededException ex)
    {
        Console.WriteLine($"  \"{display}\" => ERRO: {ex.Message}");
    }
}

static TuringMachine BuildUnaryIncrementer()
{
    var t = new Dictionary<(string, char), (string, char, char)>
    {
        { ("q0", '1'), ("q0", '1', 'R') },
        { ("q0", '_'), ("qaccept", '1', 'R') },
    };

    return new TuringMachine(t, "q0", "qaccept", "qreject");
}
