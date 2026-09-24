using System;
using System.Windows.Forms;

namespace Vibestep
{
    internal sealed class HotkeyWindow : NativeWindow, IDisposable
    {
        private const int HotkeyIdentifier = 1;

        internal HotkeyWindow()
        {
            CreateHandle(new CreateParams());
            IsRegistered = NativeMethods.RegisterHotKey(
                Handle,
                HotkeyIdentifier,
                NativeMethods.ModControl | NativeMethods.ModShift,
                NativeMethods.VkE);
        }

        internal event EventHandler Pressed;

        internal bool IsRegistered { get; private set; }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == NativeMethods.WmHotKey && message.WParam.ToInt32() == HotkeyIdentifier)
            {
                EventHandler handler = Pressed;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }

            base.WndProc(ref message);
        }

        public void Dispose()
        {
            if (IsRegistered)
            {
                NativeMethods.UnregisterHotKey(Handle, HotkeyIdentifier);
                IsRegistered = false;
            }

            DestroyHandle();
        }
    }
}

