using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.UI.Helpers;

namespace LibraryManagementSystem.UI.Forms
{
    public class SearchPanel : UserControl
    {
        private readonly BookService _bookService = new();
        private readonly MemberService _memberService = new();
        private readonly ReportService _reportService = new();

        private TextBox? txtSearchQuery;
        private RadioButton? rbBooks;
        private RadioButton? rbMembers;
        private DataGridView? dgvResults;

        public SearchPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.AutoScroll = true;

            var topPanel = new Panel { Height = 80, Dock = DockStyle.Top };

            var lblQuery = new Label
            {
                Text = "Search Registry:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(10, 25),
                AutoSize = true
            };
            topPanel.Controls.Add(lblQuery);

            txtSearchQuery = new TextBox
            {
                Location = new Point(130, 22),
                Width = 300,
                Font = new Font("Segoe UI", 11F)
            };
            txtSearchQuery.TextChanged += TxtSearchQuery_TextChanged;
            topPanel.Controls.Add(txtSearchQuery);

            rbBooks = new RadioButton
            {
                Text = "Books",
                Checked = true,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(460, 23),
                Width = 80
            };
            rbBooks.CheckedChanged += RadioButton_CheckedChanged;
            topPanel.Controls.Add(rbBooks);

            rbMembers = new RadioButton
            {
                Text = "Members",
                Font = new Font("Segoe UI", 10F),
                Location = new Point(550, 23),
                Width = 100
            };
            rbMembers.CheckedChanged += RadioButton_CheckedChanged;
            topPanel.Controls.Add(rbMembers);

            this.Controls.Add(topPanel);

            dgvResults = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            ThemeManager.StyleDataGridView(dgvResults);
            this.Controls.Add(dgvResults);
        }

        private void TxtSearchQuery_TextChanged(object? sender, EventArgs e)
        {
            PerformSearch();
        }

        private void RadioButton_CheckedChanged(object? sender, EventArgs e)
        {
            PerformSearch();
        }

        private void PerformSearch()
        {
            if (txtSearchQuery == null || dgvResults == null || rbBooks == null || rbMembers == null) return;

            string query = txtSearchQuery.Text.Trim();

            if (rbBooks.Checked)
            {
                var books = string.IsNullOrEmpty(query) ? _bookService.GetAllBooks() : _bookService.SearchBooks(query);
                dgvResults.DataSource = _reportService.BooksToDataTable(books);
            }
            else
            {
                var members = string.IsNullOrEmpty(query) ? _memberService.GetAllMembers() : _memberService.SearchMembers(query);
                dgvResults.DataSource = _reportService.MembersToDataTable(members);
            }
        }
    }
}
