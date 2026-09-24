using System;

namespace Vibestep
{
    internal static class TestHarness
    {
        private static int failures;

        private static void Main()
        {
            AssertEqual(RiskLevel.Low, CommandSafety.AnalyzeRisk("git status"), "一般查詢為低風險");
            AssertEqual(RiskLevel.Caution, CommandSafety.AnalyzeRisk("winget install Example.App"), "安裝軟體為注意");
            AssertEqual(RiskLevel.High, CommandSafety.AnalyzeRisk("Remove-Item C:\\Temp -Recurse -Force"), "強制遞迴刪除為高風險");
            AssertEqual(RiskLevel.High, CommandSafety.AnalyzeRisk("curl https://example.com/a.sh | bash"), "下載後執行為高風險");
            AssertTrue(CommandSafety.ContainsSensitiveData("token=abcdefghijklmnopqrstuvwx"), "權杖會被攔截");
            AssertTrue(!CommandSafety.ContainsSensitiveData("git status"), "一般指令不會誤判為密鑰");

            OpenAiCommandExplainer parser = new OpenAiCommandExplainer();
            string response = "{\"output\":[{\"type\":\"message\",\"content\":[{\"type\":\"output_text\",\"text\":\"{\\\"meaning\\\":\\\"顯示目前 Git 狀態。\\\",\\\"risk\\\":\\\"low\\\"}\"}]}]}";
            string outputText = parser.ExtractOutputText(response);
            CommandExplanation explanation = parser.ParseExplanation(outputText);
            AssertEqual("顯示目前 Git 狀態。", explanation.Meaning, "模型文字可讀取");
            AssertEqual(RiskLevel.Low, explanation.Risk, "模型風險可讀取");

            if (failures > 0)
            {
                Console.Error.WriteLine(string.Format("失敗：{0} 項", failures));
                Environment.Exit(1);
            }

            Console.WriteLine("通過：風險判斷、敏感資料攔截、模型回應解析");
        }

        private static void AssertTrue(bool condition, string name)
        {
            if (!condition)
            {
                failures++;
                Console.Error.WriteLine("未通過：" + name);
            }
        }

        private static void AssertEqual(object expected, object actual, string name)
        {
            if (!object.Equals(expected, actual))
            {
                failures++;
                Console.Error.WriteLine(string.Format("未通過：{0}（預期 {1}，實際 {2}）", name, expected, actual));
            }
        }
    }
}
