using LibraryManagementSystem.Models;
using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class AddEditBookForm : Form
    {
        private readonly BookService _bookService = new();
        private readonly Book? _book;

        private RoundedTextBox? txtTitle;
        private RoundedTextBox? txtAuthor;
        private RoundedTextBox? txtISBN;
        private ComboBox? cbCategory;
        private RoundedTextBox? txtPublisher;
        private RoundedTextBox? txtQuantity;
        private RoundedTextBox? txtShelfNo;
        private RoundedTextBox? txtPublishedYear;
        private RoundedButton? btnSave;
        private RoundedButton? btnCancel;

        public AddEditBookForm(Book? book)
        {
            _book = book;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(460, 520);
            this.Text = _book == null ? "Add New Book" : "Edit Book";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ThemeManager.BgColor;

            // Form container panel
            var panel = new RoundedPanel
            {
                Size = new Size(420, 440),
                Location = new Point(12, 12),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 14,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor
            };

            // Form controls
            int startY = 20;
            int spacing = 50;

            CreateLabel(panel, "Book Title", 20, startY);
            txtTitle = CreateTextBox(panel, 140, startY, 250);

            CreateLabel(panel, "Author", 20, startY + spacing);
            txtAuthor = CreateTextBox(panel, 140, startY + spacing, 250);

            CreateLabel(panel, "ISBN", 20, startY + (spacing * 2));
            txtISBN = CreateTextBox(panel, 140, startY + (spacing * 2), 250);

            CreateLabel(panel, "Category", 20, startY + (spacing * 3));
            cbCategory = new ComboBox { Location = new Point(140, startY + (spacing * 3)), Width = 250, DropDownStyle = ComboBoxStyle.DropDown };
            cbCategory.Items.AddRange(new string[] { "Fiction", "Non-Fiction", "Self Help", "Programming", "Computer Science", "Finance", "History", "Science" });
            panel.Controls.Add(cbCategory);

            CreateLabel(panel, "Publisher", 20, startY + (spacing * 4));
            txtPublisher = CreateTextBox(panel, 140, startY + (spacing * 4), 250);

            CreateLabel(panel, "Quantity", 20, startY + (spacing * 5));
            txtQuantity = CreateTextBox(panel, 140, startY + (spacing * 5), 80);

            CreateLabel(panel, "Shelf No", 20, startY + (spacing * 6));
            txtShelfNo = CreateTextBox(panel, 140, startY + (spacing * 6), 80);

            CreateLabel(panel, "Publish Year", 230, startY + (spacing * 5));
            txtPublishedYear = CreateTextBox(panel, 310, startY + (spacing * 5), 80);

            // Populating if Edit mode
            if (_book != null)
            {
                txtTitle.Text = _book.Title;
                txtAuthor.Text = _book.Author;
                txtISBN.Text = _book.ISBN;
                cbCategory.Text = _book.Category;
                txtPublisher.Text = _book.Publisher;
                txtQuantity.Text = _book.Quantity.ToString();
                txtShelfNo.Text = _book.ShelfNo;
                txtPublishedYear.Text = _book.PublishedYear.ToString();
            }
            else
            {
                txtQuantity.Text = "1";
                txtPublishedYear.Text = DateTime.Now.Year.ToString();
            }

            // Save button
            btnSave = new RoundedButton
            {
                Text = "Save Book",
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(140, 380),
                Size = new Size(120, 36),
                CornerRadius = 8
            };
            btnSave.Click += BtnSave_Click;
            panel.Controls.Add(btnSave);

            // Cancel button
            btnCancel = new RoundedButton
            {
                Text = "Cancel",
                BgColor = ThemeManager.AccentColor,
                TextColor = ThemeManager.TextColor,
                HoverColor = ThemeManager.BorderColor,
                Location = new Point(270, 380),
                Size = new Size(100, 36),
                CornerRadius = 8
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            panel.Controls.Add(btnCancel);

            this.Controls.Add(panel);
            ThemeManager.ApplyTheme(this);
        }

        private void CreateLabel(Panel p, string text, int x, int y)
        {
            var lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = ThemeManager.TextColor,
                Location = new Point(x, y + 8),
                AutoSize = true
            };
            p.Controls.Add(lbl);
        }

        private RoundedTextBox CreateTextBox(Panel p, int x, int y, int width)
        {
            var txt = new RoundedTextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 38)
            };
            p.Controls.Add(txt);
            return txt;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (txtTitle == null || txtAuthor == null || txtISBN == null || cbCategory == null ||
                txtPublisher == null || txtQuantity == null || txtShelfNo == null || txtPublishedYear == null) return;

            int qty = 0;
            int year = 0;
            int.TryParse(txtQuantity.Text, out qty);
            int.TryParse(txtPublishedYear.Text, out year);

            var book = _book ?? new Book();
            book.Title = txtTitle.Text.Trim();
            book.Author = txtAuthor.Text.Trim();
            book.ISBN = txtISBN.Text.Trim();
            book.Category = cbCategory.Text.Trim();
            book.Publisher = txtPublisher.Text.Trim();
            book.Quantity = qty;
            book.ShelfNo = txtShelfNo.Text.Trim();
            book.PublishedYear = year;

            var result = _book == null ? _bookService.AddBook(book) : _bookService.UpdateBook(book);

            if (result.Success)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(result.Message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
