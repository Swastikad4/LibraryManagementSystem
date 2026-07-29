// ============================================================
// RoundedPanel.cs — Custom Rounded Panel Control
// Library Management System — UI Controls
// ============================================================
// A panel with rounded corners, used for card-based layouts.
// Supports optional border and shadow effect.
// ============================================================

using System.Drawing.Drawing2D;

namespace LibraryManagementSystem.UI.Controls
{
    /// <summary>
    /// Custom panel with rounded corners for modern card designs.
    /// Used for dashboard cards, form containers, etc.
    /// </summary>
    public class RoundedPanel : Panel
    {
        private int _cornerRadius = 12;
        private Color _borderColor = Color.Transparent;
        private int _borderWidth = 0;

        /// <summary>Corner radius for the rounded edges.</summary>
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value; Invalidate(); }
        }

        /// <summary>Border color of the panel.</summary>
        public Color BorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        /// <summary>Border width in pixels.</summary>
        public int BorderWidth
        {
            get => _borderWidth;
            set { _borderWidth = value; Invalidate(); }
        }

        public RoundedPanel()
        {
            // Enable double buffering for smooth rendering
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);

            // Default padding for content spacing
            Padding = new Padding(16);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Create the rounded rectangle path
            var rect = new Rectangle(
                _borderWidth / 2,
                _borderWidth / 2,
                Width - _borderWidth - 1,
                Height - _borderWidth - 1);

            using var path = RoundedButton.CreateRoundedRectangle(rect, _cornerRadius);

            // Set the region to clip child controls to the rounded shape
            Region = new Region(path);

            // Fill the background
            using var bgBrush = new SolidBrush(BackColor);
            g.FillPath(bgBrush, path);

            // Draw the border if specified
            if (_borderWidth > 0 && _borderColor != Color.Transparent)
            {
                using var pen = new Pen(_borderColor, _borderWidth);
                g.DrawPath(pen, path);
            }
        }
    }
}
