using LibraryManagementSystem.Web.DataAccess;
using LibraryManagementSystem.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services and Session
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register Application Services
builder.Services.AddSingleton<BookService>();
builder.Services.AddSingleton<MemberService>();
builder.Services.AddSingleton<IssueReturnService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddSingleton<MagazineService>();
builder.Services.AddSingleton<NewspaperService>();

var app = builder.Build();

// Initialize SQLite Database
DatabaseHelper.InitializeDatabase();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "defaultWithDashboard",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run("http://localhost:5000");
