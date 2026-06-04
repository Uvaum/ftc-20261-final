namespace Parte3;

public sealed class StepLimitExceededException : Exception
{
    public int StepLimit { get; }

    public StepLimitExceededException(int stepLimit)
        : base($"Limite de {stepLimit} passos excedido. A máquina pode estar em loop infinito.")
    {
        StepLimit = stepLimit;
    }
}
