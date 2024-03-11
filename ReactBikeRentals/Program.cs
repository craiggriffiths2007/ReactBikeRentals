using Microsoft.EntityFrameworkCore;
using ReactBikes.Data;
using ReactBikes.Models;
using Microsoft.AspNetCore.Identity;
using ReactBikes.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ReactBikesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReactBikesContext") ?? throw new InvalidOperationException("Connection string 'ReactBikesContext' not found.")));

builder.Services.AddIdentity<ReactBikesUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

})
           .AddDefaultTokenProviders()
           .AddDefaultUI()
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<ReactBikesContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IBikeService, BikeService>();

builder.Services.AddControllersWithViews();

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

app.UseRouting();
app.UseAuthentication(); ;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Add admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ReactBikesContext>();
    var userManager = services.GetRequiredService<UserManager<ReactBikesUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await ReactBikesDBInitialize.AddRolesAsync(userManager, roleManager);
    await ReactBikesDBInitialize.AddManagerAsync(userManager, roleManager);
}

app.UseSwagger();
//Simple REST
app.MapPost("/create", (Bike bike, IBikeService service, ReactBikesContext _context) => Create(_context, bike, service));
app.MapGet("/get", (int id, IBikeService service, ReactBikesContext _context) => Get(_context, id, service));
app.MapGet("/list", (IBikeService service, ReactBikesContext _context) => List(_context, service));
app.MapPut("/update", (Bike bike, IBikeService service, ReactBikesContext _context) => Update(_context, bike, service));
app.MapDelete("/delete", (int id, IBikeService service, ReactBikesContext _context) => Delete(_context, id, service));

static IResult Create(ReactBikesContext _context, Bike bike, IBikeService service)
{
    return Results.Ok(service.Create(_context, bike));
}
static IResult Get(ReactBikesContext _context, int id, IBikeService service)
{
    return Results.Ok(service.Get(_context, id));
}
static IResult List(ReactBikesContext _context, IBikeService service)
{
    return Results.Ok(service.List(_context));
}
static IResult Update(ReactBikesContext _context, Bike bike, IBikeService service)
{
    return Results.Ok(service.Update(_context, bike));
}
static IResult Delete(ReactBikesContext _context, int id, IBikeService service)
{
    return Results.Ok(service.Delete(_context, id));
}

app.UseSwaggerUI();

app.Run();
