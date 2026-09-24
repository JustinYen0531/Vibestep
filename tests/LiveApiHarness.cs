using System;

namespace Vibestep
{
    internal static class LiveApiHarness
    {
        private static void Main()
        {
            try
            {
                OpenAiCommandExplainer explainer = new OpenAiCommandExplainer();
                CommandExplanation result = explainer.ExplainAsync("git status").GetAwaiter().GetResult();
                Console.WriteLine("API 連線成功");
                Console.WriteLine("解釋：" + result.Meaning);
                Console.WriteLine("風險：" + result.Risk);
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("API 連線失敗：" + exception.Message.Trim());
                Environment.Exit(1);
            }
        }
    }
}
