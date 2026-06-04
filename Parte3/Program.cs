using Parte3;

PrintHeader();
RunL4Tests();
RunIncrementerTests();

static void PrintHeader()
{
    Console.WriteLine("=======================================================");
    Console.WriteLine("  Fundamentos Teoricos da Computacao -- Parte 3");
    Console.WriteLine("  Implementacao de Maquinas de Turing em C#");
    Console.WriteLine("=======================================================");
    Console.WriteLine();
}

static void RunL4Tests()
{
    Console.WriteLine("--- MT 1: Reconhecedor de L4 = { a^n b^n c^n | n >= 1 } ---");
    Console.WriteLine();

    var mt = BuildL4Recognizer();

    var cases = new (string Input, bool Expected)[]
    {
        ("abc",       true),
        ("aabbcc",    true),
        ("aaabbbccc", true),
        ("",          false),
        ("a",         false),
        ("ab",        false),
        ("aabbc",     false),
        ("aabbccc",   false),
        ("ba",        false),
        ("abbc",      false),
    };

    foreach (var (input, expected) in cases)
        PrintL4Result(mt, input, expected);

    Console.WriteLine();
}

static void PrintL4Result(TuringMachine mt, string input, bool expected)
{
    var display = input.Length == 0 ? "(vazio)" : input;
    try
    {
        var result = mt.Run(input);
        var verdict = result.Accepted ? "ACEITA" : "REJEITA";
        var ok = result.Accepted == expected ? "[OK]  " : "[FAIL]";
        Console.WriteLine($"{ok} \"{display,-12}\" => {verdict,-8} ({result.StepsExecuted} passos)");
    }
    catch (StepLimitExceededException ex)
    {
        Console.WriteLine($"[ERR] \"{display,-12}\" => {ex.Message}");
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

static void RunIncrementerTests()
{
    Console.WriteLine("--- MT 2: Incrementador Unario f(n) = n + 1 ---");
    Console.WriteLine();

    var mt = BuildUnaryIncrementer();

    var cases = new (string Input, int ExpectedOnes)[]
    {
        ("1",    2),
        ("11",   3),
        ("111",  4),
        ("1111", 5),
        ("",     1),
    };

    foreach (var (input, expectedOnes) in cases)
    {
        var display = input.Length == 0 ? "(vazio)" : input;
        var result = mt.Run(input);
        var actualOnes = result.FinalTapeContent.Count(c => c == '1');
        var ok = actualOnes == expectedOnes ? "[OK]  " : "[FAIL]";
        Console.WriteLine($"{ok} f({input.Length}) = {actualOnes}  fita: [{result.FinalTapeContent}]  ({result.StepsExecuted} passos)");
    }

    Console.WriteLine();
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
