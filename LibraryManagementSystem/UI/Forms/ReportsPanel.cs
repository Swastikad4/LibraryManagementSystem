using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;
using System.Data;

namespace LibraryManagementSystem.UI.Forms
{
    public class ReportsPanel : UserControl
    {
        private readonly ReportService _reportService = new();

        private ComboBox? cbReportType;
        private DataGridView? dgvReport;
        private RoundedButton? btnExportPDF;
        private RoundedButton? btnExportExcel;

        public ReportsPanel()
        {
            InitializeComponent();
            cbReportType!.SelectedIndex = 0;
        }

        private void InitializeComponent()
        {
            this.AutoScroll = true;

            var topPanel = new Panel { Height = 65, Dock = DockStyle.Top };

            var lblType = new Label
            {
                Text = "Select Report Category:",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Location = new Point(10, 20),
                AutoSize = true
            };
            topPanel.Controls.Add(lblType);

            cbReportType = new ComboBox
            {
                Location = new Point(175, 17),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbReportType.Items.AddRange(new string[] { "Available Books", "Borrowed Books", "Overdue Books", "Members Registry" });
            cbReportType.SelectedIndexChanged += CbReportType_SelectedIndexChanged;
            topPanel.Controls.Add(cbReportType);

            btnExportExcel = new RoundedButton
            {
                Text = "Export Excel",
                Icon = IconHelper.Export,
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(400, 14),
                Size = new Size(130, 36),
                CornerRadius = 6
            };
            btnExportExcel.Click += BtnExportExcel_Click;
            topPanel.Controls.Add(btnExportExcel);

            btnExportPDF = new RoundedButton
            {
                Text = "Export PDF",
                Icon = IconHelper.Print,
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(540, 14),
                Size = new Size(130, 36),
                CornerRadius = 6
            };
            btnExportPDF.Click += BtnExportPDF_Click;
            topPanel.Controls.Add(btnExportPDF);

            this.Controls.Add(topPanel);

            dgvReport = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            ThemeManager.StyleDataGridView(dgvReport);
            this.Controls.Add(dgvReport);
        }

        private void CbReportType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (cbReportType == null || dgvReport == null) return;

            string selected = cbReportType.SelectedItem?.ToString() ?? "";
            dgvReport.DataSource = GetSelectedReportData(selected);
        }

        private DataTable GetSelectedReportData(string reportType)
        {
            return reportType switch
            {
                "Available Books" => _reportService.BooksToDataTable(_reportService.GetAvailableBooksReport()),
                "Borrowed Books" => _reportService.IssuedBooksToDataTable(_reportService.GetBorrowedBooksReport()),
                "Overdue Books" => _reportService.IssuedBooksToDataTable(_reportService.GetOverdueBooksReport()),
                "Members Registry" => _reportService.MembersToDataTable(_reportService.GetMembersReport()),
                _ => new DataTable()
            };
        }

        private void BtnExportExcel_Click(object? sender, EventArgs e)
        {
            if (cbReportType == null || dgvReport == null) return;

            using var sfd = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                FileName = $"{cbReportType.SelectedItem?.ToString()?.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var dt = GetSelectedReportData(cbReportType.SelectedItem?.ToString() ?? "");
                var result = _reportService.ExportToExcel(dt, sfd.FileName, cbReportType.SelectedItem?.ToString() ?? "Sheet");
                MessageBox.Show(result.Message, result.Success ? "Success" : "Error");
            }
        }

        private void BtnExportPDF_Click(object? sender, EventArgs e)
        {
            if (cbReportType == null || dgvReport == null) return;

            using var sfd = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"{cbReportType.SelectedItem?.ToString()?.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var dt = GetSelectedReportData(cbReportType.SelectedItem?.ToString() ?? "");
                var result = _reportService.ExportToPdf(dt, sfd.FileName, cbReportType.SelectedItem?.ToString() ?? "Report");
                MessageBox.Show(result.Message, result.Success ? "Success" : "Error");
            }
        }
    }
}
