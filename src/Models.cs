namespace Vibestep
{
    internal enum RiskLevel
    {
        Low = 0,
        Caution = 1,
        High = 2
    }

    internal sealed class CommandExplanation
    {
        internal CommandExplanation(string meaning, RiskLevel risk)
        {
            Meaning = meaning;
            Risk = risk;
        }

        internal string Meaning { get; private set; }

        internal RiskLevel Risk { get; private set; }
    }
}

