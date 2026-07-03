using LiveNewsAggregator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// إصلاح مشكلة undefined
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddSingleton<NewsGeneratorService>();
builder.Services.AddSingleton<NewsRankingService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<NewsGeneratorService>());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "news",
    pattern: "news/{action=Index}/{id?}");

app.Run();