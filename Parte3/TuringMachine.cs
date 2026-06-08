namespace Parte3;

public sealed class TuringMachine
{
    private const char Blank = '_';

    private readonly Dictionary<(string State, char Symbol), (string NextState, char WriteSymbol, char Direction)> _transitions;
    private readonly string _initialState;
    private readonly string _acceptState;
    private readonly string _rejectState;
    private readonly int _maxSteps;

    public TuringMachine(
        Dictionary<(string State, char Symbol), (string NextState, char WriteSymbol, char Direction)> transitions,
        string initialState,
        string acceptState,
        string rejectState,
        int maxSteps = 10_000)
    {
        _transitions = transitions;
        _initialState = initialState;
        _acceptState = acceptState;
        _rejectState = rejectState;
        _maxSteps = maxSteps;
    }

    // onStep(passo, estado, fita_com_cabecote, posicao_cabecote)
    public TuringMachineResult Run(string input, Action<int, string, string, int>? onStep = null)
    {
        var tape = LoadTape(input);
        var state = _initialState;
        var head = 0;
        var steps = 0;

        while (state != _acceptState && state != _rejectState)
        {
            if (steps >= _maxSteps)
                throw new StepLimitExceededException(_maxSteps);

            onStep?.Invoke(steps, state, BuildTapeDisplay(tape, head), head);

            var symbol = tape.TryGetValue(head, out var s) ? s : Blank;

            if (!_transitions.TryGetValue((state, symbol), out var transition))
            {
                state = _rejectState;
                break;
            }

            var (nextState, writeSymbol, direction) = transition;
            tape[head] = writeSymbol;
            state = nextState;
            head += direction == 'R' ? 1 : -1;
            steps++;
        }

        onStep?.Invoke(steps, state, BuildTapeDisplay(tape, head), head);

        return new TuringMachineResult(state == _acceptState, steps, BuildTapeSnapshot(tape));
    }

    private static string BuildTapeDisplay(Dictionary<int, char> tape, int head)
    {
        var positions = tape.Keys.Append(head);
        var min = positions.Min();
        var max = positions.Max();
        var result = "";
        for (var i = min; i <= max; i++)
        {
            var c = tape.TryGetValue(i, out var v) ? v : Blank;
            result += i == head ? $"[{c}]" : c.ToString();
        }
        return result;
    }

    private static Dictionary<int, char> LoadTape(string input)
    {
        var tape = new Dictionary<int, char>();
        for (var i = 0; i < input.Length; i++)
            tape[i] = input[i];
        return tape;
    }

    private static string BuildTapeSnapshot(Dictionary<int, char> tape)
    {
        if (tape.Count == 0)
            return string.Empty;

        var min = tape.Keys.Min();
        var max = tape.Keys.Max();
        return string.Concat(Enumerable.Range(min, max - min + 1)
            .Select(i => tape.TryGetValue(i, out var c) ? c : Blank));
    }
}
