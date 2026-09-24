using System.Text.RegularExpressions;

namespace Vibestep
{
    internal static class CommandSafety
    {
        private static readonly Regex[] SensitivePatterns =
        {
            new Regex(@"sk-[A-Za-z0-9_-]{20,}", RegexOptions.Compiled),
            new Regex(@"gh[pousr]_[A-Za-z0-9]{20,}", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"AKIA[0-9A-Z]{16}", RegexOptions.Compiled),
            new Regex(@"(api[_-]?key|token|password|passwd|secret)\s*[:=]\s*[\""']?[^\s\""']{8,}", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"authorization\s*:\s*bearer\s+\S+", RegexOptions.Compiled | RegexOptions.IgnoreCase)
        };

        private static readonly Regex[] HighRiskPatterns =
        {
            new Regex(@"\brm\s+[^\r\n]*-[^\r\n]*r[^\r\n]*f|\brm\s+[^\r\n]*-[^\r\n]*f[^\r\n]*r", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\bremove-item\b[^\r\n]*-recurse[^\r\n]*-force", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\b(del|erase)\b[^\r\n]*/[sq]", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\b(format|diskpart|reg\s+delete|bcdedit)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\b(shutdown|restart-computer|stop-computer)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\b(iex|invoke-expression)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"(curl|wget|invoke-webrequest|iwr)[^\r\n]*(\||;)\s*(sh|bash|powershell|pwsh)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\bgit\s+push\b[^\r\n]*(--force|-f)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase)
        };

        private static readonly Regex[] CautionPatterns =
        {
            new Regex(@"\b(sudo|runas)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\b(winget|choco|scoop)\s+install\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\b(npm|pnpm|yarn)\b[^\r\n]*(install|add)[^\r\n]*(-g|--global)", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\bpip\s+install\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\b(curl|wget|invoke-webrequest|iwr)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\bgit\s+clean\b[^\r\n]*-[^\r\n]*f", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\bdocker\s+system\s+prune\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\bset-executionpolicy\b", RegexOptions.Compiled | RegexOptions.IgnoreCase),
            new Regex(@"\bchmod\s+777\b", RegexOptions.Compiled | RegexOptions.IgnoreCase)
        };

        internal static bool ContainsSensitiveData(string text)
        {
            return MatchesAny(text, SensitivePatterns);
        }

        internal static RiskLevel AnalyzeRisk(string command)
        {
            if (MatchesAny(command, HighRiskPatterns))
            {
                return RiskLevel.High;
            }

            if (MatchesAny(command, CautionPatterns))
            {
                return RiskLevel.Caution;
            }

            return RiskLevel.Low;
        }

        internal static RiskLevel Max(RiskLevel first, RiskLevel second)
        {
            return first >= second ? first : second;
        }

        private static bool MatchesAny(string text, Regex[] patterns)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            foreach (Regex pattern in patterns)
            {
                if (pattern.IsMatch(text))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

