using System.Drawing.Drawing2D;
using LibraryManagementSystem.UI.Helpers;

namespace LibraryManagementSystem.UI.Controls
{
    public class StatCard : UserControl
    {
        private string _title = "Total Books";
        private string _value = "0";
        private string _icon = "📚";
        private Color _accentColor;
        private bool _isHovered = false;

        public string Title
        {
            get => _title;
            set { _title = value; Invalidate(); }
        }

        public string Value
        {
            get => _value;
            set { _value = value; Invalidate(); }
        }

        public string Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        public Color AccentColor
        {
            get => _accentColor;
            set { _accentColor = value; Invalidate(); }
        }

        public StatCard()
        {
            _accentColor = ThemeManager.AccentColor;

            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);

            Size = new Size(240, 100);
            Cursor = Cursors.Hand;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(0, _isHovered ? 0 : 2, Width - 1, Height - 3);
            using var path = RoundedButton.CreateRoundedRectangle(rect, 12);

            using var bgBrush = new SolidBrush(ThemeManager.CardColor);
            g.FillPath(bgBrush, path);

            using var borderPen = new Pen(ThemeManager.BorderColor, 1);
            g.DrawPath(borderPen, path);

            // Title
            using var titleFont = new Font("Segoe UI", 8F, FontStyle.Bold);
            using var titleBrush = new SolidBrush(ThemeManager.TextSecondaryColor);
            g.DrawString(_title.ToUpper(), titleFont, titleBrush, new PointF(16, 14));

            // Value
            using var valueFont = new Font("Segoe UI", 22F, FontStyle.Bold);
            using var valueBrush = new SolidBrush(ThemeManager.TextColor);
            g.DrawString(_value, valueFont, valueBrush, new PointF(14, 38));

            // Icon square on the right
            int iconSize = 40;
            int iconX = Width - iconSize - 16;
            int iconY = (Height - iconSize) / 2;

            var iconRect = new Rectangle(iconX, iconY, iconSize, iconSize);
            using var iconPath = RoundedButton.CreateRoundedRectangle(iconRect, 8);
            using var iconBgBrush = new SolidBrush(_accentColor);
            g.FillPath(iconBgBrush, iconPath);

            using var iconFont = new Font("Segoe UI Emoji", 15F);
            var iconTextSize = g.MeasureString(_icon, iconFont);
            float iconTextX = iconX + (iconSize - iconTextSize.Width) / 2;
            float iconTextY = iconY + (iconSize - iconTextSize.Height) / 2;
            g.DrawString(_icon, iconFont, new SolidBrush(ThemeManager.TextColor), iconTextX, iconTextY);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }
    }
}
