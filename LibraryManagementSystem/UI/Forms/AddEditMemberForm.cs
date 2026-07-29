using LibraryManagementSystem.Models;
using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class AddEditMemberForm : Form
    {
        private readonly MemberService _memberService = new();
        private readonly Member? _member;

        private RoundedTextBox? txtName;
        private RoundedTextBox? txtEmail;
        private RoundedTextBox? txtPhone;
        private RoundedTextBox? txtAddress;
        private RoundedButton? btnSave;
        private RoundedButton? btnCancel;

        public AddEditMemberForm(Member? member)
        {
            _member = member;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(400, 360);
            this.Text = _member == null ? "Add Member" : "Edit Member";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ThemeManager.BgColor;

            var panel = new RoundedPanel
            {
                Size = new Size(360, 280),
                Location = new Point(12, 12),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 14,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor
            };

            int startY = 20;
            int spacing = 50;

            CreateLabel(panel, "Name", 20, startY);
            txtName = CreateTextBox(panel, 100, startY, 230);

            CreateLabel(panel, "Email", 20, startY + spacing);
            txtEmail = CreateTextBox(panel, 100, startY + spacing, 230);

            CreateLabel(panel, "Phone", 20, startY + (spacing * 2));
            txtPhone = CreateTextBox(panel, 100, startY + (spacing * 2), 230);

            CreateLabel(panel, "Address", 20, startY + (spacing * 3));
            txtAddress = CreateTextBox(panel, 100, startY + (spacing * 3), 230);

            if (_member != null)
            {
                txtName.Text = _member.Name;
                txtEmail.Text = _member.Email;
                txtPhone.Text = _member.Phone;
                txtAddress.Text = _member.Address;
            }

            btnSave = new RoundedButton
            {
                Text = "Save",
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(100, 220),
                Size = new Size(110, 36),
                CornerRadius = 8
            };
            btnSave.Click += BtnSave_Click;
            panel.Controls.Add(btnSave);

            btnCancel = new RoundedButton
            {
                Text = "Cancel",
                BgColor = ThemeManager.AccentColor,
                TextColor = ThemeManager.TextColor,
                HoverColor = ThemeManager.BorderColor,
                Location = new Point(220, 220),
                Size = new Size(110, 36),
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
            if (txtName == null || txtEmail == null || txtPhone == null || txtAddress == null) return;

            var member = _member ?? new Member();
            member.Name = txtName.Text.Trim();
            member.Email = txtEmail.Text.Trim();
            member.Phone = txtPhone.Text.Trim();
            member.Address = txtAddress.Text.Trim();

            var result = _member == null ? _memberService.AddMember(member) : _memberService.UpdateMember(member);
            if (result.Success)
            {
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show(result.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
