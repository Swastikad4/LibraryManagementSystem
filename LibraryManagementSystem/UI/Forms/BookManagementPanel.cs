using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class BookManagementPanel : UserControl
    {
        private readonly MainForm _mainForm;
        private readonly BookService _bookService = new();
        private readonly ReportService _reportService = new();

        private DataGridView? dgvBooks;
        private TextBox? txtSearch;
        private ComboBox? cbCategoryFilter;
        private RoundedButton? btnAddBook;
        private RoundedButton? btnReset;

        public BookManagementPanel(MainForm mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.AutoScroll = true;

            // Action bar at top
            var topActionPanel = new Panel
            {
                Height = 60,
                Dock = DockStyle.Top
            };

            btnAddBook = new RoundedButton
            {
                Text = "Add New Book",
                Icon = IconHelper.Add,
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(10, 10),
                Size = new Size(160, 38),
                CornerRadius = 8
            };
            btnAddBook.Click += BtnAddBook_Click;
            topActionPanel.Controls.Add(btnAddBook);

            // Search Label & Box
            var lblSearch = new Label
            {
                Text = "Search:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(200, 20),
                AutoSize = true
            };
            topActionPanel.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(255, 17),
                Width = 200,
                Font = new Font("Segoe UI", 10F)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            topActionPanel.Controls.Add(txtSearch);

            // Category filter
            var lblCategory = new Label
            {
                Text = "Category:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(480, 20),
                AutoSize = true
            };
            topActionPanel.Controls.Add(lblCategory);

            cbCategoryFilter = new ComboBox
            {
                Location = new Point(550, 17),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbCategoryFilter.SelectedIndexChanged += CbCategoryFilter_SelectedIndexChanged;
            topActionPanel.Controls.Add(cbCategoryFilter);

            btnReset = new RoundedButton
            {
                Text = "Reset",
                BgColor = ThemeManager.AccentColor,
                TextColor = ThemeManager.TextColor,
                HoverColor = ThemeManager.BorderColor,
                Location = new Point(720, 14),
                Size = new Size(80, 32),
                CornerRadius = 6
            };
            btnReset.Click += BtnReset_Click;
            topActionPanel.Controls.Add(btnReset);

            this.Controls.Add(topActionPanel);

            // DataGridView layout
            dgvBooks = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            ThemeManager.StyleDataGridView(dgvBooks);
            dgvBooks.CellContentClick += DgvBooks_CellContentClick;
            this.Controls.Add(dgvBooks);
        }

        private void LoadData()
        {
            if (dgvBooks == null) return;

            var books = _bookService.GetAllBooks();
            dgvBooks.DataSource = _reportService.BooksToDataTable(books);

            // Populate category filter if empty
            if (cbCategoryFilter != null && cbCategoryFilter.Items.Count <= 1)
            {
                cbCategoryFilter.Items.Clear();
                cbCategoryFilter.Items.Add("All Categories");
                var cats = _bookService.GetCategories();
                foreach (var cat in cats)
                {
                    cbCategoryFilter.Items.Add(cat);
                }
                cbCategoryFilter.SelectedIndex = 0;
            }

            // Create action column buttons
            SetupActionColumns();
        }

        private void SetupActionColumns()
        {
            if (dgvBooks == null) return;

            // Remove existing action columns if present
            if (dgvBooks.Columns.Contains("EditCol")) dgvBooks.Columns.Remove("EditCol");
            if (dgvBooks.Columns.Contains("DeleteCol")) dgvBooks.Columns.Remove("DeleteCol");
            if (dgvBooks.Columns.Contains("BorrowCol")) dgvBooks.Columns.Remove("BorrowCol");

            var editCol = new DataGridViewButtonColumn
            {
                Name = "EditCol",
                HeaderText = "Actions",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            dgvBooks.Columns.Add(editCol);

            var deleteCol = new DataGridViewButtonColumn
            {
                Name = "DeleteCol",
                HeaderText = "",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            dgvBooks.Columns.Add(deleteCol);

            var borrowCol = new DataGridViewButtonColumn
            {
                Name = "BorrowCol",
                HeaderText = "",
                Text = "Borrow/Issue",
                UseColumnTextForButtonValue = true,
                Width = 100
            };
            dgvBooks.Columns.Add(borrowCol);
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            if (txtSearch == null || dgvBooks == null) return;
            string term = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(term))
            {
                LoadData();
            }
            else
            {
                var filtered = _bookService.SearchBooks(term);
                dgvBooks.DataSource = _reportService.BooksToDataTable(filtered);
            }
        }

        private void CbCategoryFilter_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbCategoryFilter == null || dgvBooks == null) return;
            string cat = cbCategoryFilter.SelectedItem?.ToString() ?? "All Categories";
            if (cat == "All Categories")
            {
                LoadData();
            }
            else
            {
                var filtered = _bookService.GetBooksByCategory(cat);
                dgvBooks.DataSource = _reportService.BooksToDataTable(filtered);
            }
        }

        private void BtnReset_Click(object? sender, EventArgs e)
        {
            if (txtSearch != null) txtSearch.Text = "";
            if (cbCategoryFilter != null) cbCategoryFilter.SelectedIndex = 0;
            LoadData();
        }

        private void BtnAddBook_Click(object? sender, EventArgs e)
        {
            using var form = new AddEditBookForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
                NotificationToast.Show(_mainForm, "Book added successfully!", NotificationType.Success);
            }
        }

        private void DgvBooks_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (dgvBooks == null || e.RowIndex < 0) return;

            int bookId = Convert.ToInt32(dgvBooks.Rows[e.RowIndex].Cells["BookID"].Value);
            string title = dgvBooks.Rows[e.RowIndex].Cells["Title"].Value.ToString() ?? "";

            if (dgvBooks.Columns[e.ColumnIndex].Name == "EditCol")
            {
                var book = _bookService.GetBookById(bookId);
                if (book != null)
                {
                    using var form = new AddEditBookForm(book);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                        NotificationToast.Show(_mainForm, "Book updated successfully!", NotificationType.Success);
                    }
                }
            }
            else if (dgvBooks.Columns[e.ColumnIndex].Name == "DeleteCol")
            {
                var confirm = MessageBox.Show($"Are you sure you want to delete '{title}'?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    var result = _bookService.DeleteBook(bookId);
                    if (result.Success)
                    {
                        LoadData();
                        NotificationToast.Show(_mainForm, result.Message, NotificationType.Success);
                    }
                    else
                    {
                        NotificationToast.Show(_mainForm, result.Message, NotificationType.Error);
                    }
                }
            }
            else if (dgvBooks.Columns[e.ColumnIndex].Name == "BorrowCol")
            {
                // Navigate to borrow screen
                _mainForm.LoadPanel("issue");
            }
        }
    }
}
