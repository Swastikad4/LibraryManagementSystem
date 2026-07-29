using LibraryManagementSystem.BusinessLogic;
using LibraryManagementSystem.UI.Helpers;
using LibraryManagementSystem.UI.Controls;

namespace LibraryManagementSystem.UI.Forms
{
    public class DashboardPanel : UserControl
    {
        private readonly MainForm _mainForm;
        private readonly BookService _bookService = new();
        private readonly MemberService _memberService = new();
        private readonly IssueReturnService _issueReturnService = new();

        private Label? lblWelcome;
        private Label? lblTimeInfo;
        private StatCard? cardTotalBooks;
        private StatCard? cardAvailableBooks;
        private StatCard? cardBorrowedBooks;
        private StatCard? cardTotalMembers;
        private StatCard? cardDueToday;
        private StatCard? cardOverdue;

        public DashboardPanel(MainForm mainForm)
        {
            _mainForm = mainForm;
            InitializeComponent();
            RefreshStats();
        }

        private void InitializeComponent()
        {
            this.DoubleBuffered = true;
            this.AutoScroll = true;

            // Welcome Text
            lblWelcome = new Label
            {
                Text = $"Welcome back, {AuthService.CurrentUser?.FullName ?? "Admin"}!",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            this.Controls.Add(lblWelcome);

            lblTimeInfo = new Label
            {
                Text = $"Here is the current overview of your Library System.",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = ThemeManager.TextSecondaryColor,
                Location = new Point(12, 40),
                AutoSize = true
            };
            this.Controls.Add(lblTimeInfo);

            // FlowLayout for stat cards
            var cardGrid = new FlowLayoutPanel
            {
                Location = new Point(10, 70),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            cardTotalBooks = new StatCard { Title = "Total Books", Value = "0", Icon = IconHelper.TotalBooks, AccentColor = ThemeManager.AccentColor };
            cardAvailableBooks = new StatCard { Title = "Available Books", Value = "0", Icon = IconHelper.AvailableBooks, AccentColor = ThemeManager.AccentColor };
            cardBorrowedBooks = new StatCard { Title = "Borrowed Books", Value = "0", Icon = IconHelper.BorrowedBooks, AccentColor = ThemeManager.AccentColor };
            cardTotalMembers = new StatCard { Title = "Total Members", Value = "0", Icon = IconHelper.TotalMembers, AccentColor = ThemeManager.AccentColor };
            cardDueToday = new StatCard { Title = "Due Today", Value = "0", Icon = IconHelper.DueToday, AccentColor = ThemeManager.AccentColor };
            cardOverdue = new StatCard { Title = "Overdue Books", Value = "0", Icon = IconHelper.OverdueBooks, AccentColor = ThemeManager.AccentColor };

            // Give each card a margin for spacing inside the FlowLayoutPanel
            foreach (var card in new[] { cardTotalBooks, cardAvailableBooks, cardBorrowedBooks, cardTotalMembers, cardDueToday, cardOverdue })
            {
                card.Margin = new Padding(5);
            }

            cardGrid.Controls.AddRange(new Control[] {
                cardTotalBooks, cardAvailableBooks, cardBorrowedBooks,
                cardTotalMembers, cardDueToday, cardOverdue
            });
            this.Controls.Add(cardGrid);

            // Quick Actions section
            var lblActions = new Label
            {
                Text = "Quick Actions",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Location = new Point(12, 300),
                AutoSize = true
            };
            this.Controls.Add(lblActions);

            var actionGrid = new FlowLayoutPanel
            {
                Location = new Point(10, 330),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // Card for Books Collection
            var panelBooks = new RoundedPanel
            {
                Size = new Size(350, 120),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 12,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor,
                Margin = new Padding(5)
            };
            var lblBookTitle = new Label
            {
                Text = "📖 Books Collection",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(15, 15),
                AutoSize = true
            };
            var lblBookDesc = new Label
            {
                Text = "View, edit, and manage the\nmain library catalog.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = ThemeManager.TextSecondaryColor,
                Location = new Point(15, 40),
                AutoSize = true
            };
            var btnManageBooks = new RoundedButton
            {
                Text = "Manage Books →",
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(15, 80),
                Size = new Size(160, 32),
                CornerRadius = 6
            };
            btnManageBooks.Click += (s, e) => _mainForm.LoadPanel("books");
            panelBooks.Controls.AddRange(new Control[] { lblBookTitle, lblBookDesc, btnManageBooks });

            // Card for Borrow Records
            var panelBorrow = new RoundedPanel
            {
                Size = new Size(350, 120),
                BackColor = ThemeManager.CardColor,
                CornerRadius = 12,
                BorderWidth = 1,
                BorderColor = ThemeManager.BorderColor,
                Margin = new Padding(5)
            };
            var lblBorrowTitle = new Label
            {
                Text = "⏳ Borrow Records",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(15, 15),
                AutoSize = true
            };
            var lblBorrowDesc = new Label
            {
                Text = "Track book issues, returns,\nand calculate active fine balances.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = ThemeManager.TextSecondaryColor,
                Location = new Point(15, 40),
                AutoSize = true
            };
            var btnManageRecords = new RoundedButton
            {
                Text = "Manage Records →",
                BgColor = ThemeManager.PrimaryColor,
                HoverColor = ThemeManager.HoverColor,
                Location = new Point(15, 80),
                Size = new Size(160, 32),
                CornerRadius = 6
            };
            btnManageRecords.Click += (s, e) => _mainForm.LoadPanel("return");
            panelBorrow.Controls.AddRange(new Control[] { lblBorrowTitle, lblBorrowDesc, btnManageRecords });

            actionGrid.Controls.AddRange(new Control[] { panelBooks, panelBorrow });
            this.Controls.Add(actionGrid);

            // Reposition quick actions section when stat cards resize
            cardGrid.SizeChanged += (s, e) =>
            {
                int newY = cardGrid.Bottom + 15;
                lblActions.Location = new Point(12, newY);
                actionGrid.Location = new Point(10, newY + 30);
            };
        }

        private void RefreshStats()
        {
            if (cardTotalBooks == null || cardAvailableBooks == null || cardBorrowedBooks == null ||
                cardTotalMembers == null || cardDueToday == null || cardOverdue == null) return;

            cardTotalBooks.Value = _bookService.GetTotalCount().ToString();
            cardAvailableBooks.Value = _bookService.GetAvailableCount().ToString();
            cardBorrowedBooks.Value = _issueReturnService.GetBorrowedCount().ToString();
            cardTotalMembers.Value = _memberService.GetTotalCount().ToString();
            cardDueToday.Value = _issueReturnService.GetDueTodayCount().ToString();
            cardOverdue.Value = _issueReturnService.GetOverdueCount().ToString();
        }
    }
}
