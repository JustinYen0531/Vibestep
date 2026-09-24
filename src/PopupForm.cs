using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Vibestep
{
    internal sealed class PopupForm : Form
    {
        private readonly Panel accentPanel;
        private readonly Label meaningLabel;
        private readonly Label riskLabel;
        private readonly Timer closeTimer;
        private IntPtr previousWindow;

        internal PopupForm()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(25, 28, 38);
            ClientSize = new Size(390, 108);
            KeyPreview = true;
            Font = new Font("Microsoft JhengHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

            accentPanel = new Panel();
            accentPanel.Dock = DockStyle.Left;
            accentPanel.Width = 6;
            accentPanel.BackColor = Color.FromArgb(104, 211, 145);
            Controls.Add(accentPanel);

            meaningLabel = new Label();
            meaningLabel.AutoEllipsis = true;
            meaningLabel.ForeColor = Color.FromArgb(244, 246, 252);
            meaningLabel.Font = new Font("Microsoft JhengHei UI", 11.5F, FontStyle.Bold, GraphicsUnit.Point);
            meaningLabel.Location = new Point(24, 18);
            meaningLabel.Size = new Size(344, 48);
            meaningLabel.Text = "正在解釋…";
            Controls.Add(meaningLabel);

            riskLabel = new Label();
            riskLabel.AutoSize = true;
            riskLabel.ForeColor = Color.FromArgb(104, 211, 145);
            riskLabel.Font = new Font("Microsoft JhengHei UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point);
            riskLabel.Location = new Point(24, 76);
            riskLabel.Text = "● 風險：判斷中";
            Controls.Add(riskLabel);

            closeTimer = new Timer();
            closeTimer.Interval = 12000;
            closeTimer.Tick += delegate
            {
                closeTimer.Stop();
                Hide();
            };

            Deactivate += delegate
            {
                closeTimer.Stop();
                Hide();
            };

            KeyDown += delegate(object sender, KeyEventArgs args)
            {
                if (args.KeyCode == Keys.Escape)
                {
                    args.Handled = true;
                    DismissAndReturn();
                }
            };

            Resize += delegate { UpdateRoundedRegion(); };
            Click += delegate { DismissAndReturn(); };
            meaningLabel.Click += delegate { DismissAndReturn(); };
            riskLabel.Click += delegate { DismissAndReturn(); };
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int dropShadow = 0x00020000;
                CreateParams parameters = base.CreateParams;
                parameters.ClassStyle |= dropShadow;
                return parameters;
            }
        }

        internal void ShowLoading(Point cursor, IntPtr foregroundWindow)
        {
            previousWindow = foregroundWindow;
            accentPanel.BackColor = Color.FromArgb(98, 168, 255);
            meaningLabel.Text = "正在解釋這段指令…";
            riskLabel.ForeColor = Color.FromArgb(155, 190, 255);
            riskLabel.Text = "● 風險：判斷中";
            ShowNear(cursor);
        }

        internal void ShowExplanation(Point cursor, IntPtr foregroundWindow, CommandExplanation explanation)
        {
            previousWindow = foregroundWindow;
            meaningLabel.Text = explanation.Meaning;
            ApplyRisk(explanation.Risk);
            ShowNear(cursor);
            closeTimer.Stop();
            closeTimer.Start();
        }

        internal void ShowNotice(Point cursor, IntPtr foregroundWindow, string message, RiskLevel risk)
        {
            ShowExplanation(cursor, foregroundWindow, new CommandExplanation(message, risk));
        }

        private void ShowNear(Point cursor)
        {
            Screen screen = Screen.FromPoint(cursor);
            Rectangle area = screen.WorkingArea;
            int x = cursor.X + 16;
            int y = cursor.Y - Height - 16;

            if (x + Width > area.Right)
            {
                x = area.Right - Width - 12;
            }

            if (x < area.Left)
            {
                x = area.Left + 12;
            }

            if (y < area.Top)
            {
                y = cursor.Y + 22;
            }

            if (y + Height > area.Bottom)
            {
                y = area.Bottom - Height - 12;
            }

            Location = new Point(x, y);
            closeTimer.Stop();

            if (!Visible)
            {
                Show();
            }

            Activate();
        }

        private void ApplyRisk(RiskLevel risk)
        {
            Color color;
            string text;

            if (risk == RiskLevel.High)
            {
                color = Color.FromArgb(255, 100, 112);
                text = "● 風險：高";
            }
            else if (risk == RiskLevel.Caution)
            {
                color = Color.FromArgb(255, 195, 88);
                text = "● 風險：注意";
            }
            else
            {
                color = Color.FromArgb(104, 211, 145);
                text = "● 風險：低";
            }

            accentPanel.BackColor = color;
            riskLabel.ForeColor = color;
            riskLabel.Text = text;
        }

        private void DismissAndReturn()
        {
            closeTimer.Stop();
            Hide();
            if (previousWindow != IntPtr.Zero)
            {
                NativeMethods.SetForegroundWindow(previousWindow);
            }
        }

        private void UpdateRoundedRegion()
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = 14;
                Rectangle bounds = new Rectangle(0, 0, Width, Height);
                path.AddArc(bounds.Left, bounds.Top, radius, radius, 180, 90);
                path.AddArc(bounds.Right - radius, bounds.Top, radius, radius, 270, 90);
                path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(bounds.Left, bounds.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();
                Region = new Region(path);
            }
        }
    }
}

