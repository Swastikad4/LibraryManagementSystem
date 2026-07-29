using LibraryManagementSystem.UI.Helpers;
using System.Drawing.Drawing2D;

namespace LibraryManagementSystem.UI.Controls
{
    public class SidebarMenuItem
    {
        public string Icon { get; set; } = "";
        public string Text { get; set; } = "";
        public string Key { get; set; } = "";
    }

    public class SidebarMenu : Panel
    {
        private List<SidebarMenuItem> _items = new();
        private int _activeIndex = 0;
        private int _hoverIndex = -1;
        private int _itemHeight = 44;
        private int _headerHeight = 70;

        public event Action<string>? MenuItemClicked;

        public int ActiveIndex
        {
            get => _activeIndex;
            set { _activeIndex = value; Invalidate(); }
        }

        public SidebarMenu()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.OptimizedDoubleBuffer, true);

            Width = 220;
            Dock = DockStyle.Left;
            BackColor = ThemeManager.SidebarColor;
            Cursor = Cursors.Hand;
        }

        public void SetItems(List<SidebarMenuItem> items)
        {
            _items = items;
            Invalidate();
        }

        public void SetActiveByKey(string key)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Key == key)
                {
                    _activeIndex = i;
                    Invalidate();
                    return;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            using var bgBrush = new SolidBrush(ThemeManager.SidebarColor);
            g.FillRectangle(bgBrush, ClientRectangle);

            DrawHeader(g);

            for (int i = 0; i < _items.Count; i++)
            {
                DrawMenuItem(g, i);
            }
        }

        private void DrawHeader(Graphics g)
        {
            int logoSize = 34;
            int logoX = 16;
            int logoY = (_headerHeight - logoSize) / 2;

            var logoRect = new Rectangle(logoX, logoY, logoSize, logoSize);
            using var logoPath = RoundedButton.CreateRoundedRectangle(logoRect, 6);
            using var logoBrush = new SolidBrush(Color.White);
            g.FillPath(logoBrush, logoPath);

            using var logoFont = new Font("Segoe UI Emoji", 14F);
            g.DrawString("📚", logoFont, new SolidBrush(ThemeManager.SidebarColor), logoX + 2, logoY + 3);

            using var nameFont = new Font("Segoe UI", 12F, FontStyle.Bold);
            using var nameBrush = new SolidBrush(Color.White);
            g.DrawString("LMS", nameFont, nameBrush, logoX + logoSize + 10, logoY + 5);

            using var dividerPen = new Pen(Color.FromArgb(40, 255, 255, 255));
            g.DrawLine(dividerPen, 12, _headerHeight - 1, Width - 12, _headerHeight - 1);
        }

        private void DrawMenuItem(Graphics g, int index)
        {
            var item = _items[index];
            int y = _headerHeight + (index * _itemHeight);
            bool isActive = index == _activeIndex;
            bool isHovered = index == _hoverIndex;

            if (isActive)
            {
                using var activeBar = new SolidBrush(Color.White);
                g.FillRectangle(activeBar, 0, y + 6, 4, _itemHeight - 12);
            }

            if (isActive || isHovered)
            {
                Color bgColor = isActive ? Color.FromArgb(30, 255, 255, 255) : Color.FromArgb(15, 255, 255, 255);
                var itemRect = new Rectangle(6, y + 2, Width - 12, _itemHeight - 4);
                using var itemPath = RoundedButton.CreateRoundedRectangle(itemRect, 6);
                using var itemBrush = new SolidBrush(bgColor);
                g.FillPath(itemBrush, itemPath);
            }

            using var iconFont = new Font("Segoe UI Emoji", 11F);
            Color textColor = isActive ? Color.White : Color.FromArgb(210, 255, 255, 255);
            using var iconBrush = new SolidBrush(textColor);
            g.DrawString(item.Icon, iconFont, iconBrush, 16, y + 10);

            using var textFont = new Font("Segoe UI", 9F, isActive ? FontStyle.Bold : FontStyle.Regular);
            using var textBrush = new SolidBrush(textColor);
            g.DrawString(item.Text, textFont, textBrush, 44, y + 11);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int newHover = GetItemIndexAtPoint(e.Location);
            if (newHover != _hoverIndex)
            {
                _hoverIndex = newHover;
                Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hoverIndex = -1;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            int clickedIndex = GetItemIndexAtPoint(e.Location);
            if (clickedIndex >= 0 && clickedIndex < _items.Count)
            {
                _activeIndex = clickedIndex;
                Invalidate();
                MenuItemClicked?.Invoke(_items[clickedIndex].Key);
            }
            base.OnMouseClick(e);
        }

        private int GetItemIndexAtPoint(Point point)
        {
            if (point.Y < _headerHeight) return -1;
            int index = (point.Y - _headerHeight) / _itemHeight;
            return (index >= 0 && index < _items.Count) ? index : -1;
        }
    }
}
