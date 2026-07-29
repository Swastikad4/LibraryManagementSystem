// ============================================================
// RoundedButton.cs — Custom Rounded Button Control
// Library Management System — UI Controls
// ============================================================
// A modern button with rounded corners and hover effects.
// Uses GDI+ GraphicsPath for smooth rounded rectangle drawing.
// ============================================================

using System.Drawing.Drawing2D;
using LibraryManagementSystem.UI.Helpers;

namespace LibraryManagementSystem.UI.Controls
{
    /// <summary>
    /// Custom button control with rounded corners, hover effects,
    /// and smooth color transitions. Provides a modern UI look.
    /// </summary>
    public class RoundedButton : Control
    {
        // ---- Properties ----
        private Color _bgColor;
        private Color _hoverColor;
        private Color _textColor = Color.White;
        private int _cornerRadius = 12;
        private bool _isHovered = false;
        private string _icon = "";

        /// <summary>Background color of the button.</summary>
        public Color BgColor
        {
            get => _bgColor;
            set { _bgColor = value; Invalidate(); }
        }

        /// <summary>Color when the mouse hovers over the button.</summary>
        public Color HoverColor
        {
            get => _hoverColor;
            set { _hoverColor = value; Invalidate(); }
        }

        /// <summary>Text color of the button.</summary>
        public Color TextColor
        {
            get => _textColor;
            set { _textColor = value; Invalidate(); }
        }

        /// <summary>Corner radius for rounded edges.</summary>
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value; Invalidate(); }
        }

        /// <summary>Optional Unicode icon displayed before text.</summary>
        public string Icon
        {
            get => _icon;
            set { _icon = value; Invalidate(); }
        }

        /// <summary>
        /// Creates a new RoundedButton with default Rose/Mauve colors.
        /// </summary>
        public RoundedButton()
        {
            // Set default colors from theme
            _bgColor = ThemeManager.PrimaryColor;
            _hoverColor = ThemeManager.HoverColor;

            // Enable custom painting
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);

            // Default size
            Size = new Size(160, 42);
            Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            Cursor = Cursors.Hand;
        }

        // ---- Paint Override ----

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Determine current color based on hover state
            Color currentBg = _isHovered ? _hoverColor : _bgColor;

            // Create rounded rectangle path
            using var path = CreateRoundedRectangle(
                new Rectangle(0, 0, Width - 1, Height - 1), _cornerRadius);

            // Fill background
            using var brush = new SolidBrush(currentBg);
            g.FillPath(brush, path);

            // Draw text (with optional icon)
            string displayText = string.IsNullOrEmpty(_icon)
                ? Text
                : $"{_icon}  {Text}";

            using var textBrush = new SolidBrush(_textColor);
            var textFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.DrawString(displayText, Font, textBrush,
                new RectangleF(0, 0, Width, Height), textFormat);
        }

        // ---- Mouse Event Handlers ----

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate(); // Trigger repaint with hover color
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate(); // Trigger repaint with normal color
            base.OnMouseLeave(e);
        }

        // ---- Helper Methods ----

        /// <summary>
        /// Creates a GraphicsPath representing a rounded rectangle.
        /// </summary>
        /// <param name="rect">The bounding rectangle.</param>
        /// <param name="radius">Corner radius.</param>
        /// <returns>A GraphicsPath with rounded corners.</returns>
        public static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int diameter = radius * 2;

            // Top-left arc
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            // Top-right arc
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            // Bottom-right arc
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            // Bottom-left arc
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
