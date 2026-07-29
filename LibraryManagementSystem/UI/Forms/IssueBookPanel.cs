using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class IssueBookPanel : UserControl
    {
        private readonly BookService _bookService = new();
        private readonly MemberService _memberService = new();
        private readonly IssueReturnService _issueReturnService = new();

        private TextBox? txtBookSearch;
        private TextBox? txtMemberSearch;
        private ListBox? lstBooks;
        private ListBox? lstMembers;
        private Label? lblSelectedBook;
        private Label? lblSelectedMember;
        private RoundedButton? btnIssue;

        private int selectedBookId = 0;
        private int selectedMemberId = 0;

        public IssueBookPanel()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.AutoScroll = true;

            // Panel arrangement
            var pnlLeft = new RoundedPanel
            {
                Size = new Size(380, 450),
                Location = new Point(10, 10),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 12,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor
            };

            var lblBookHead = new Label { Text = "1. Search & Select Book", Font = new Font("Segoe UI", 11F, FontStyle.Bold), Location = new Point(15, 15), AutoSize = true };
            txtBookSearch = new TextBox { Location = new Point(15, 45), Width = 350, Font = new Font("Segoe UI", 10F) };
            txtBookSearch.TextChanged += TxtBookSearch_TextChanged;
            lstBooks = new ListBox { Location = new Point(15, 80), Size = new Size(350, 350), Font = new Font("Segoe UI", 9.5F) };
            lstBooks.SelectedIndexChanged += LstBooks_SelectedIndexChanged;

            pnlLeft.Controls.AddRange(new Control[] { lblBookHead, txtBookSearch, lstBooks });
            this.Controls.Add(pnlLeft);

            var pnlRight = new RoundedPanel
            {
                Size = new Size(380, 450),
                Location = new Point(400, 10),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 12,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor
            };

            var lblMemberHead = new Label { Text = "2. Search & Select Member", Font = new Font("Segoe UI", 11F, FontStyle.Bold), Location = new Point(15, 15), AutoSize = true };
            txtMemberSearch = new TextBox { Location = new Point(15, 45), Width = 350, Font = new Font("Segoe UI", 10F) };
            txtMemberSearch.TextChanged += TxtMemberSearch_TextChanged;
            lstMembers = new ListBox { Location = new Point(15, 80), Size = new Size(350, 350), Font = new Font("Segoe UI", 9.5F) };
            lstMembers.SelectedIndexChanged += LstMembers_SelectedIndexChanged;

            pnlRight.Controls.AddRange(new Control[] { lblMemberHead, txtMemberSearch, lstMembers });
            this.Controls.Add(pnlRight);

            // Selection details panel (Bottom)
            var pnlBottom = new RoundedPanel
            {
                Size = new Size(770, 100),
                Location = new Point(10, 470),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 12,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor
            };

            lblSelectedBook = new Label { Text = "Selected Book: None", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            lblSelectedMember = new Label { Text = "Selected Member: None", Font = new Font("Segoe UI", 10F, FontStyle.Bold), Location = new Point(20, 50), AutoSize = true };

            btnIssue = new RoundedButton
            {
                Text = "Issue Book",
                Icon = IconHelper.IssueBook,
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(570, 30),
                Size = new Size(180, 42),
                CornerRadius = 8,
                Enabled = false
            };
            btnIssue.Click += BtnIssue_Click;

            pnlBottom.Controls.AddRange(new Control[] { lblSelectedBook, lblSelectedMember, btnIssue });
            this.Controls.Add(pnlBottom);
        }

        private void TxtBookSearch_TextChanged(object? sender, EventArgs e)
        {
            if (txtBookSearch == null || lstBooks == null) return;
            lstBooks.Items.Clear();
            var books = _bookService.SearchBooks(txtBookSearch.Text.Trim());
            foreach (var b in books)
            {
                lstBooks.Items.Add($"{b.BookID} | {b.Title} (Avail: {b.AvailableCopies})");
            }
        }

        private void TxtMemberSearch_TextChanged(object? sender, EventArgs e)
        {
            if (txtMemberSearch == null || lstMembers == null) return;
            lstMembers.Items.Clear();
            var members = _memberService.SearchMembers(txtMemberSearch.Text.Trim());
            foreach (var m in members)
            {
                lstMembers.Items.Add($"{m.MemberID} | {m.Name} ({m.Email})");
            }
        }

        private void LstBooks_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstBooks == null || lstBooks.SelectedItem == null || lblSelectedBook == null) return;
            string sel = lstBooks.SelectedItem.ToString() ?? "";
            string idStr = sel.Split('|')[0].Trim();
            if (int.TryParse(idStr, out int id))
            {
                selectedBookId = id;
                lblSelectedBook.Text = $"Selected Book: {sel.Split('|')[1].Trim()}";
                CheckSelection();
            }
        }

        private void LstMembers_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstMembers == null || lstMembers.SelectedItem == null || lblSelectedMember == null) return;
            string sel = lstMembers.SelectedItem.ToString() ?? "";
            string idStr = sel.Split('|')[0].Trim();
            if (int.TryParse(idStr, out int id))
            {
                selectedMemberId = id;
                lblSelectedMember.Text = $"Selected Member: {sel.Split('|')[1].Trim()}";
                CheckSelection();
            }
        }

        private void CheckSelection()
        {
            if (btnIssue != null)
            {
                btnIssue.Enabled = selectedBookId > 0 && selectedMemberId > 0;
            }
        }

        private void BtnIssue_Click(object? sender, EventArgs e)
        {
            if (selectedBookId == 0 || selectedMemberId == 0) return;

            var result = _issueReturnService.IssueBook(selectedBookId, selectedMemberId);
            MessageBox.Show(result.Message, result.Success ? "Success" : "Error");

            if (result.Success)
            {
                // Reset panel state
                selectedBookId = 0;
                selectedMemberId = 0;
                if (lblSelectedBook != null) lblSelectedBook.Text = "Selected Book: None";
                if (lblSelectedMember != null) lblSelectedMember.Text = "Selected Member: None";
                if (txtBookSearch != null) txtBookSearch.Text = "";
                if (txtMemberSearch != null) txtMemberSearch.Text = "";
                if (lstBooks != null) lstBooks.Items.Clear();
                if (lstMembers != null) lstMembers.Items.Clear();
                CheckSelection();
            }
        }
    }
}
