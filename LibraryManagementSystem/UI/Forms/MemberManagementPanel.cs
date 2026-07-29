using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class MemberManagementPanel : UserControl
    {
        private readonly MemberService _memberService = new();
        private readonly ReportService _reportService = new();

        private DataGridView? dgvMembers;
        private TextBox? txtSearch;
        private RoundedButton? btnAddMember;

        public MemberManagementPanel()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.AutoScroll = true;

            var topActionPanel = new Panel { Height = 60, Dock = DockStyle.Top };

            btnAddMember = new RoundedButton
            {
                Text = "Add Registry Member",
                Icon = IconHelper.Add,
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(10, 10),
                Size = new Size(200, 38),
                CornerRadius = 8
            };
            btnAddMember.Click += BtnAddMember_Click;
            topActionPanel.Controls.Add(btnAddMember);

            var lblSearch = new Label
            {
                Text = "Search Registry:",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(240, 20),
                AutoSize = true
            };
            topActionPanel.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(350, 17),
                Width = 220,
                Font = new Font("Segoe UI", 10F)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            topActionPanel.Controls.Add(txtSearch);

            this.Controls.Add(topActionPanel);

            dgvMembers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true
            };
            ThemeManager.StyleDataGridView(dgvMembers);
            dgvMembers.CellContentClick += DgvMembers_CellContentClick;
            this.Controls.Add(dgvMembers);
        }

        private void LoadData()
        {
            if (dgvMembers == null) return;
            var members = _memberService.GetAllMembers();
            dgvMembers.DataSource = _reportService.MembersToDataTable(members);
            SetupActionColumns();
        }

        private void SetupActionColumns()
        {
            if (dgvMembers == null) return;
            if (dgvMembers.Columns.Contains("EditCol")) dgvMembers.Columns.Remove("EditCol");
            if (dgvMembers.Columns.Contains("DeleteCol")) dgvMembers.Columns.Remove("DeleteCol");

            var editCol = new DataGridViewButtonColumn
            {
                Name = "EditCol",
                HeaderText = "Actions",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            dgvMembers.Columns.Add(editCol);

            var deleteCol = new DataGridViewButtonColumn
            {
                Name = "DeleteCol",
                HeaderText = "",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            dgvMembers.Columns.Add(deleteCol);
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            if (txtSearch == null || dgvMembers == null) return;
            string term = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(term))
            {
                LoadData();
            }
            else
            {
                var filtered = _memberService.SearchMembers(term);
                dgvMembers.DataSource = _reportService.MembersToDataTable(filtered);
            }
        }

        private void BtnAddMember_Click(object? sender, EventArgs e)
        {
            using var form = new AddEditMemberForm(null);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void DgvMembers_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (dgvMembers == null || e.RowIndex < 0) return;

            int memberId = Convert.ToInt32(dgvMembers.Rows[e.RowIndex].Cells["MemberID"].Value);
            string name = dgvMembers.Rows[e.RowIndex].Cells["Name"].Value.ToString() ?? "";

            if (dgvMembers.Columns[e.ColumnIndex].Name == "EditCol")
            {
                var member = _memberService.GetMemberById(memberId);
                if (member != null)
                {
                    using var form = new AddEditMemberForm(member);
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadData();
                    }
                }
            }
            else if (dgvMembers.Columns[e.ColumnIndex].Name == "DeleteCol")
            {
                var confirm = MessageBox.Show($"Delete member '{name}'?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    var result = _memberService.DeleteMember(memberId);
                    MessageBox.Show(result.Message, result.Success ? "Success" : "Error");
                    if (result.Success) LoadData();
                }
            }
        }
    }
}
