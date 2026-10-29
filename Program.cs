var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizePage("/Index", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Admins", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Orphans", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Volunteers", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Sponsors", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Donations", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Items", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Events", "AdminOnly");
    options.Conventions.AuthorizeFolder("/MealPlans", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Schools", "AdminOnly");
    options.Conventions.AuthorizeFolder("/EducationRecords", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Assignments", "AdminOnly");
    options.Conventions.AuthorizeFolder("/SponsorPortal", "SponsorOnly");
    options.Conventions.AuthorizePage("/Account/ChangePassword", "AnyUser");
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Logout");
});

builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Login";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SponsorOnly", policy => policy.RequireRole("Sponsor"));
    options.AddPolicy("AnyUser", policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddScoped<OrphanageWeb.Data.OrphanRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.VolunteerRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.SponsorRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.AdminRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.AccountRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.DonationRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.ItemRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.EventRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.MealplanRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.SchoolRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.EducationRecordRepository>();
builder.Services.AddScoped<OrphanageWeb.Data.AssignmentRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
