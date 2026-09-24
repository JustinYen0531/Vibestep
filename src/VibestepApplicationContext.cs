using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vibestep
{
    internal sealed class VibestepApplicationContext : ApplicationContext
    {
        private readonly HotkeyWindow hotkeyWindow;
        private readonly NotifyIcon trayIcon;
        private readonly PopupForm popup;
        private readonly OpenAiCommandExplainer explainer;
        private bool isBusy;

        internal VibestepApplicationContext()
        {
            popup = new PopupForm();
            explainer = new OpenAiCommandExplainer();
            hotkeyWindow = new HotkeyWindow();
            hotkeyWindow.Pressed += HandleHotkey;

            ContextMenuStrip menu = new ContextMenuStrip();
            menu.Items.Add("使用方式", null, ShowHelp);
            menu.Items.Add("API Key 狀態", null, ShowApiKeyStatus);
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("離開 Vibestep", null, ExitApplication);

            trayIcon = new NotifyIcon();
            trayIcon.Icon = SystemIcons.Information;
            trayIcon.Text = "Vibestep 指令解釋";
            trayIcon.ContextMenuStrip = menu;
            trayIcon.Visible = true;
            trayIcon.DoubleClick += ShowHelp;

            if (!hotkeyWindow.IsRegistered)
            {
                MessageBox.Show(
                    "Ctrl + Shift + E 已被其他程式使用，Vibestep 無法註冊快捷鍵。",
                    "Vibestep",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                ExitApplication(this, EventArgs.Empty);
                return;
            }

            trayIcon.BalloonTipTitle = "Vibestep 已啟動";
            trayIcon.BalloonTipText = "反白終端機指令後，按 Ctrl + Shift + E。";
            trayIcon.ShowBalloonTip(3500);
        }

        private async void HandleHotkey(object sender, EventArgs args)
        {
            if (isBusy)
            {
                return;
            }

            isBusy = true;
            Point cursor = Cursor.Position;
            IntPtr previousWindow = NativeMethods.GetForegroundWindow();

            try
            {
                string selectedText = await ClipboardCapture.CaptureSelectedTextAsync();
                if (string.IsNullOrWhiteSpace(selectedText))
                {
                    popup.ShowNotice(cursor, previousWindow, "沒有取得反白文字，請重新選取後再試。", RiskLevel.Caution);
                    return;
                }

                if (selectedText.Length > 6000)
                {
                    popup.ShowNotice(cursor, previousWindow, "選取內容太長，請縮小到單一指令。", RiskLevel.Caution);
                    return;
                }

                popup.ShowLoading(cursor, previousWindow);
                CommandExplanation explanation = await explainer.ExplainAsync(selectedText);
                popup.ShowExplanation(cursor, previousWindow, explanation);
            }
            catch (Exception exception)
            {
                popup.ShowNotice(cursor, previousWindow, FriendlyError(exception), RiskLevel.Caution);
            }
            finally
            {
                isBusy = false;
            }
        }

        private static string FriendlyError(Exception exception)
        {
            if (exception is InvalidOperationException && !string.IsNullOrWhiteSpace(exception.Message))
            {
                return exception.Message.Trim();
            }

            if (exception is TaskCanceledException)
            {
                return "等待模型回應逾時，請稍後再試。";
            }

            return "暫時無法取得解釋，請確認網路後再試。";
        }

        private void ShowHelp(object sender, EventArgs args)
        {
            MessageBox.Show(
                "1. 在終端機反白一段指令。\r\n" +
                "2. 按 Ctrl + Shift + E。\r\n" +
                "3. 查看滑鼠右上方的白話解釋與風險。\r\n\r\n" +
                "Vibestep 不會執行指令。",
                "Vibestep 使用方式",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void ShowApiKeyStatus(object sender, EventArgs args)
        {
            string message = explainer.HasApiKey()
                ? "OPENAI_API_KEY 已設定。"
                : "尚未找到 OPENAI_API_KEY。";

            MessageBox.Show(message, "Vibestep", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExitApplication(object sender, EventArgs args)
        {
            popup.Hide();
            trayIcon.Visible = false;
            trayIcon.Dispose();
            hotkeyWindow.Dispose();
            ExitThread();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                popup.Dispose();
                trayIcon.Dispose();
                hotkeyWindow.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}

