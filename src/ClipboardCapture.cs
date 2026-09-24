using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vibestep
{
    internal static class ClipboardCapture
    {
        internal static async Task<string> CaptureSelectedTextAsync()
        {
            IDataObject originalData = null;
            bool hadOriginalData = false;

            try
            {
                await WaitForHotkeyReleaseAsync();
                originalData = TryGetClipboardData();
                hadOriginalData = originalData != null;
                TryClearClipboard();

                SendKeys.SendWait("^c");
                await Task.Delay(180);

                string selectedText = TryGetClipboardText();
                return selectedText == null ? string.Empty : selectedText.Trim();
            }
            finally
            {
                if (hadOriginalData)
                {
                    TryRestoreClipboard(originalData);
                }
                else
                {
                    TryClearClipboard();
                }
            }
        }

        private static async Task WaitForHotkeyReleaseAsync()
        {
            const int virtualControl = 0x11;
            const int virtualShift = 0x10;

            for (int attempt = 0; attempt < 25; attempt++)
            {
                bool controlPressed = (NativeMethods.GetAsyncKeyState(virtualControl) & 0x8000) != 0;
                bool shiftPressed = (NativeMethods.GetAsyncKeyState(virtualShift) & 0x8000) != 0;
                if (!controlPressed && !shiftPressed)
                {
                    return;
                }

                await Task.Delay(20);
            }
        }

        private static IDataObject TryGetClipboardData()
        {
            for (int attempt = 0; attempt < 4; attempt++)
            {
                try
                {
                    return Clipboard.GetDataObject();
                }
                catch (ExternalException)
                {
                    System.Threading.Thread.Sleep(40);
                }
            }

            return null;
        }

        private static string TryGetClipboardText()
        {
            for (int attempt = 0; attempt < 4; attempt++)
            {
                try
                {
                    return Clipboard.ContainsText() ? Clipboard.GetText() : string.Empty;
                }
                catch (ExternalException)
                {
                    System.Threading.Thread.Sleep(40);
                }
            }

            return string.Empty;
        }

        private static void TryRestoreClipboard(IDataObject data)
        {
            for (int attempt = 0; attempt < 4; attempt++)
            {
                try
                {
                    Clipboard.SetDataObject(data, true);
                    return;
                }
                catch (ExternalException)
                {
                    System.Threading.Thread.Sleep(40);
                }
            }
        }

        private static void TryClearClipboard()
        {
            for (int attempt = 0; attempt < 4; attempt++)
            {
                try
                {
                    Clipboard.Clear();
                    return;
                }
                catch (ExternalException)
                {
                    System.Threading.Thread.Sleep(40);
                }
            }
        }
    }
}
