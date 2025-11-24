using MAPI.Data;
using MAPI.IServices;
using MAPI.Model;
using MAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!context.Products.Any())
    {
        context.Products.AddRange(
            new Products { Id = 1, Name = "Apple", Price = 1.20m, Status = "Available" },
            new Products { Id = 2, Name = "Banana", Price = 0.80m, Status = "Available" },
            new Products { Id = 3, Name = "Orange", Price = 1.50m, Status = "Available" }
        );
        context.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
