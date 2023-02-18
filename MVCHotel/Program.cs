using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MVCHotel.Motels;
using MVCHotel.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
//var connectionString = builder.Configuration.GetConnectionString("MVCHotelContextConnection") ?? throw new InvalidOperationException("Connection string 'MVCHotelContextConnection' not found.");

builder.Services.AddDbContext<MVCHotelContext>();

//builder.Services.AddDefaultIdentity<MVCHotelUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MVCHotelContext>();
builder.Services.AddIdentity<MVCHotelUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<MVCHotelContext>()

.AddDefaultTokenProviders()
   .AddDefaultUI();


// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
    options.SignIn.RequireConfirmedAccount = true;

});


//for: when bookingadd, you need login first 
builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/AccessDenied";
    options.SlidingExpiration = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



var userManager = builder.Services.BuildServiceProvider().GetService<UserManager<MVCHotelUser>>();
var roleManager = builder.Services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>();

SeedUsersAndRoles(userManager, roleManager);

app.MapRazorPages();
app.Run();

void SeedUsersAndRoles(UserManager<MVCHotelUser> userManager, RoleManager<IdentityRole> roleManager)
{
    //NOTE:we only seed roles in this particular example but 
    // you may want to seed users with assigned role too(eg. an administrator, user)
    string[] roleNameList = new string[] { "Customer", "Admin","Motel" };
    foreach (string roleName in roleNameList)
    {
        if (!roleManager.RoleExistsAsync(roleName).Result)
        {
            IdentityRole role = new IdentityRole();
            role.Name = roleName;
            IdentityResult result = roleManager.CreateAsync(role).Result;
            //warning:we ignore any errors that create may return,they should be AT least logged
        }

    }
    // warning : for testing only. do not do it on a production system!
    //create an administrator.

    string adminEmail = "admin@admin.com";
    string adminPass = "Admin123!";
    if (userManager.FindByNameAsync(adminEmail).Result == null)
    {
        MVCHotelUser user = new MVCHotelUser();
        user.UserName = adminEmail;
        user.Email = adminEmail;
        user.EmailConfirmed = true;
        IdentityResult result = userManager.CreateAsync(user, adminPass).Result;
        if (result.Succeeded)
        {
            IdentityResult result2 = userManager.AddToRoleAsync(user, "Admin").Result;
        }

    }

}


