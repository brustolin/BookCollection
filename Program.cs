using Microsoft.EntityFrameworkCore;
using BookTracker.Data;
using BookTracker.Tools.ChangeTracker;
using BookTracker.Tools.QueryComposer;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntityChangeTracker();
builder.Services.AddEntityQueryComposer();
builder.Services.AddEntityQueryComposerSwagger();

builder.Services.AddDbContext<BookCollectionDataContext>(options =>
    options.UseInMemoryDatabase("LibraryDb")
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookCollectionDataContext>();
    BookCollectionDataContext.Initialize(context);
}

app.Run();