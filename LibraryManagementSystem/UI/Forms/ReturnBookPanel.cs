using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class ReturnBookPanel : UserControl
    {
        private readonly IssueReturnService _issueReturnService = new();
        private readonly ReportService _reportService = new();

        private DataGridView? dgvIssued;
        private TextBox? txtSearch;
        private RoundedButton? btnReturn;
        private Label? lblSelectedInfo;

        private int selectedIssueId = 0;

        public ReturnBookPanel()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.AutoScroll = true;

            var topPanel = new Panel { Height = 60, Dock = DockStyle.Top };

            var lblSearch = new Label
            {
                Text = "Search Borrow Records:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(10, 20),
                AutoSize = true
            };
            topPanel.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(170, 17),
                Width = 250,
                Font = new Font("Segoe UI", 10F)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            topPanel.Controls.Add(txtSearch);

            this.Controls.Add(topPanel);

            // Bottom action panel
            var bottomPanel = new Panel { Height = 70, Dock = DockStyle.Bottom };
            lblSelectedInfo = new Label
            {
                Text = "Select an issue record from the grid above.",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(15, 25),
                AutoSize = true
            };
            bottomPanel.Controls.Add(lblSelectedInfo);

            btnReturn = new RoundedButton
            {
                Text = "Return selected book",
                Icon = IconHelper.ReturnBook,
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(620, 15),
                Size = new Size(200, 42),
                CornerRadius = 8,
                Enabled = false
            };
            btnReturn.Click += BtnReturn_Click;
            bottomPanel.Controls.Add(btnReturn);

            this.Controls.Add(bottomPanel);

            dgvIssued = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            ThemeManager.StyleDataGridView(dgvIssued);
            dgvIssued.SelectionChanged += DgvIssued_SelectionChanged;
            this.Controls.Add(dgvIssued);
        }

        private void LoadData()
        {
            if (dgvIssued == null) return;
            var activeIssues = _issueReturnService.GetCurrentlyIssued();
            dgvIssued.DataSource = _reportService.IssuedBooksToDataTable(activeIssues);
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            if (txtSearch == null || dgvIssued == null) return;
            string term = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(term))
            {
                LoadData();
            }
            else
            {
                var filtered = _issueReturnService.SearchIssued(term);
                dgvIssued.DataSource = _reportService.IssuedBooksToDataTable(filtered);
            }
        }

        private void DgvIssued_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvIssued == null || dgvIssued.SelectedRows.Count == 0 || lblSelectedInfo == null || btnReturn == null) return;

            var row = dgvIssued.SelectedRows[0];
            selectedIssueId = Convert.ToInt32(row.Cells["IssueID"].Value);
            string book = row.Cells["Book"].Value.ToString() ?? "";
            string member = row.Cells["Member"].Value.ToString() ?? "";

            lblSelectedInfo.Text = $"Selected: '{book}' borrowed by '{member}'";
            btnReturn.Enabled = true;
        }

        private void BtnReturn_Click(object? sender, EventArgs e)
        {
            if (selectedIssueId == 0) return;

            var result = _issueReturnService.ReturnBook(selectedIssueId);
            MessageBox.Show(result.Message, result.Success ? "Success" : "Error");

            if (result.Success)
            {
                LoadData();
                selectedIssueId = 0;
                if (lblSelectedInfo != null) lblSelectedInfo.Text = "Select an issue record from the grid above.";
                if (btnReturn != null) btnReturn.Enabled = false;
            }
        }
    }
}
