// ============================================================
// RoundedTextBox.cs — Custom Rounded TextBox Control
// Library Management System — UI Controls
// ============================================================
// Wraps a standard TextBox in a rounded border panel
// for a modern input field appearance.
// ============================================================

using System.Drawing.Drawing2D;
using LibraryManagementSystem.UI.Helpers;

namespace LibraryManagementSystem.UI.Controls
{
    /// <summary>
    /// Custom text input control with rounded borders.
    /// Wraps a standard TextBox inside a rounded container.
    /// </summary>
    public class RoundedTextBox : UserControl
    {
        private readonly TextBox _innerTextBox;
        private int _cornerRadius = 10;
        private Color _borderColor;
        private Color _focusBorderColor;
        private bool _isFocused = false;

        /// <summary>The actual text value.</summary>
        public override string Text
        {
            get => _innerTextBox.Text;
            set => _innerTextBox.Text = value;
        }

        /// <summary>Placeholder text shown when empty.</summary>
        public string PlaceholderText
        {
            get => _innerTextBox.PlaceholderText;
            set => _innerTextBox.PlaceholderText = value;
        }

        /// <summary>Whether the input is masked (for passwords).</summary>
        public bool IsPassword
        {
            get => _innerTextBox.UseSystemPasswordChar;
            set => _innerTextBox.UseSystemPasswordChar = value;
        }

        /// <summary>Whether the text box is read-only.</summary>
        public bool ReadOnly
        {
            get => _innerTextBox.ReadOnly;
            set => _innerTextBox.ReadOnly = value;
        }

        /// <summary>Maximum text length.</summary>
        public int MaxLength
        {
            get => _innerTextBox.MaxLength;
            set => _innerTextBox.MaxLength = value;
        }

        /// <summary>Gets the inner TextBox for event binding.</summary>
        public TextBox InnerTextBox => _innerTextBox;

        /// <summary>Corner radius.</summary>
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = value; Invalidate(); }
        }

        public RoundedTextBox()
        {
            _borderColor = ThemeManager.BorderColor;
            _focusBorderColor = ThemeManager.PrimaryColor;

            // Setup the control
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);

            Height = 42;
            Padding = new Padding(12, 0, 12, 0);

            // Create the inner TextBox
            _innerTextBox = new TextBox
            {
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10F),
                BackColor = ThemeManager.CardColor,
                ForeColor = ThemeManager.TextColor,
                Dock = DockStyle.Fill
            };

            // Track focus state for border color change
            _innerTextBox.GotFocus += (s, e) => { _isFocused = true; Invalidate(); };
            _innerTextBox.LostFocus += (s, e) => { _isFocused = false; Invalidate(); };

            // Forward text changed event
            _innerTextBox.TextChanged += (s, e) => OnTextChanged(e);

            Controls.Add(_innerTextBox);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw the rounded border
            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using var path = RoundedButton.CreateRoundedRectangle(rect, _cornerRadius);

            // Fill background
            using var bgBrush = new SolidBrush(ThemeManager.CardColor);
            g.FillPath(bgBrush, path);

            // Draw border (changes color on focus)
            Color currentBorder = _isFocused ? _focusBorderColor : _borderColor;
            using var pen = new Pen(currentBorder, _isFocused ? 2 : 1);
            g.DrawPath(pen, path);

            // Clip region
            Region = new Region(path);
        }

        /// <summary>
        /// Updates the control's theme colors.
        /// Call this when the theme changes.
        /// </summary>
        public void UpdateTheme()
        {
            _borderColor = ThemeManager.BorderColor;
            _focusBorderColor = ThemeManager.PrimaryColor;
            _innerTextBox.BackColor = ThemeManager.CardColor;
            _innerTextBox.ForeColor = ThemeManager.TextColor;
            Invalidate();
        }
    }
}
