// ============================================================
// NotificationToast.cs — Slide-in Notification Control
// Library Management System — UI Controls
// ============================================================
// Displays a temporary notification message that slides in
// from the top-right corner and auto-dismisses after 3 seconds.
// ============================================================

using System.Drawing.Drawing2D;
using LibraryManagementSystem.UI.Helpers;

namespace LibraryManagementSystem.UI.Controls
{
    /// <summary>
    /// Notification types with distinct colors and icons.
    /// </summary>
    public enum NotificationType
    {
        Success,
        Error,
        Warning,
        Info
    }

    /// <summary>
    /// A temporary toast notification that slides in from the right
    /// and auto-dismisses. Shows success, error, warning, or info messages.
    /// </summary>
    public class NotificationToast : Form
    {
        private readonly System.Windows.Forms.Timer _closeTimer;
        private readonly System.Windows.Forms.Timer _fadeTimer;
        private readonly NotificationType _type;
        private readonly string _message;

        /// <summary>
        /// Shows a notification toast on the specified parent form.
        /// </summary>
        /// <param name="parent">The parent form to show the notification on.</param>
        /// <param name="message">The notification message.</param>
        /// <param name="type">Type of notification (Success, Error, Warning, Info).</param>
        /// <param name="durationMs">How long to show the notification (default: 3000ms).</param>
        public static void Show(Form parent, string message,
            NotificationType type = NotificationType.Info, int durationMs = 3000)
        {
            var toast = new NotificationToast(message, type, durationMs);

            // Position at top-right corner of parent
            toast.StartPosition = FormStartPosition.Manual;
            toast.Location = new Point(
                parent.Location.X + parent.Width - toast.Width - 20,
                parent.Location.Y + 80);

            toast.Show(parent);
        }

        private NotificationToast(string message, NotificationType type, int durationMs)
        {
            _message = message;
            _type = type;

            // Form setup — borderless, no taskbar, transparent
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            Size = new Size(350, 70);
            BackColor = Color.White;
            Opacity = 0.95;

            // Enable custom painting
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);

            // Auto-close timer
            _closeTimer = new System.Windows.Forms.Timer { Interval = durationMs };
            _closeTimer.Tick += (s, e) =>
            {
                _closeTimer.Stop();
                StartFadeOut();
            };
            _closeTimer.Start();

            // Fade-out timer
            _fadeTimer = new System.Windows.Forms.Timer { Interval = 20 };
            _fadeTimer.Tick += (s, e) =>
            {
                Opacity -= 0.05;
                if (Opacity <= 0)
                {
                    _fadeTimer.Stop();
                    Close();
                }
            };

            // Click to dismiss
            Click += (s, e) =>
            {
                _closeTimer.Stop();
                Close();
            };
        }

        private void StartFadeOut()
        {
            _fadeTimer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Get colors based on notification type
            var (accentColor, icon) = _type switch
            {
                NotificationType.Success => (ThemeManager.SuccessColor, IconHelper.Success),
                NotificationType.Error => (ThemeManager.DangerColor, IconHelper.Error),
                NotificationType.Warning => (ThemeManager.WarningColor, IconHelper.Warning),
                _ => (ThemeManager.InfoColor, IconHelper.Info)
            };

            // Draw background with rounded corners
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedButton.CreateRoundedRectangle(rect, 12);
            using var bgBrush = new SolidBrush(ThemeManager.CardColor);
            g.FillPath(bgBrush, path);

            // Draw left accent bar
            var accentRect = new Rectangle(0, 0, 5, Height);
            using var accentPath = RoundedButton.CreateRoundedRectangle(accentRect, 2);
            using var accentBrush = new SolidBrush(accentColor);
            g.FillPath(accentBrush, accentPath);

            // Draw border
            using var borderPen = new Pen(ThemeManager.BorderColor, 1);
            g.DrawPath(borderPen, path);

            // Draw icon
            using var iconFont = new Font("Segoe UI Emoji", 16F);
            g.DrawString(icon, iconFont, new SolidBrush(accentColor), 16, 18);

            // Draw message
            using var textFont = new Font("Segoe UI", 9.5F);
            using var textBrush = new SolidBrush(ThemeManager.TextColor);
            var textRect = new RectangleF(50, 12, Width - 70, Height - 24);
            g.DrawString(_message, textFont, textBrush, textRect);

            // Draw close button (×)
            using var closeFont = new Font("Segoe UI", 12F);
            using var closeBrush = new SolidBrush(ThemeManager.TextSecondaryColor);
            g.DrawString("×", closeFont, closeBrush, Width - 28, 8);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _closeTimer.Dispose();
            _fadeTimer.Dispose();
            base.OnFormClosed(e);
        }
    }
}
