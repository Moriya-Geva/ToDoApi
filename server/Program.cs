//using Microsoft.EntityFrameworkCore;
//using TodoApi;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddDbContext<ToDoDbContext>(options => options.UseMySql
//(builder.Configuration.GetConnectionString("ToDoDB"),
//ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//builder.Services.AddCors(options => options.AddPolicy("Everything", policy =>
//{
//    policy
//    .AllowAnyHeader()
//    .AllowAnyMethod()
//    .AllowAnyOrigin();
//}));


//var app = builder.Build();
//app.UseCors("Everything");

//app.MapGet("/", async (ToDoDbContext t) => 
//{
//    return await t.Items.ToListAsync();
//});

//app.MapPost("/", async(ToDoDbContext t, Item i) =>
//{
//    t.Items.Add(i);
//    await t.SaveChangesAsync();

//    return Results.Created($"/{i.Id}", i);
//});

//app.MapPut("/{id}", async (ToDoDbContext t, int id, Item updatedItem) =>
//{
//    var existingItem = await t.Items.FindAsync(id);
//    if (existingItem == null)
//    {
//        return Results.NotFound();
//    }

//    // עדכון שדות הפריט
//    if(updatedItem.Name!=null)
//        existingItem.Name = updatedItem.Name;
//    existingItem.IsComplete = updatedItem.IsComplete;
    

//    await t.SaveChangesAsync();

//    return Results.NoContent(); 

//});


//app.MapDelete("/{id}", async (ToDoDbContext t, int id) =>
//{
//    var existingItem = await t.Items.FindAsync(id);
//    if (existingItem == null)
//    {
//        return Results.NotFound();
//    }

//    t.Items.Remove(existingItem);
//    await t.SaveChangesAsync();

//    return Results.NoContent(); 
//});

//app.UseCors("Everything");
//app.MapGet("/", () => "server API is running");
//app.Run();
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// הוספת DbContext עם MySQL
builder.Services.AddDbContext<ToDoDbContext>(options => 
    options.UseMySql(builder.Configuration.GetConnectionString("ToDoDB"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

// הוספת שירותי בקרות
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// הגדרת Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDo API", Version = "v1" });
});

// הגדרת מדיניות CORS
builder.Services.AddCors(options => options.AddPolicy("Everything", policy =>
{
    policy.AllowAnyHeader()
          .AllowAnyMethod()
          .AllowAnyOrigin();
}));

var app = builder.Build();

// הפעלת CORS
app.UseCors("Everything");

// הפעלת Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API v1");
    c.RoutePrefix = "swagger"; // כך ש-Swagger יהיה זמין ב- /swagger
});

// הגדרת מסלולים
app.MapGet("/", async (ToDoDbContext t) => await t.Items.ToListAsync())
    .WithTags("ToDo Items")
    .WithDescription("מקבל את כל המשימות");

app.MapPost("/", async (ToDoDbContext t, Item i) =>
{
    t.Items.Add(i);
    await t.SaveChangesAsync();
    return Results.Created($"/{i.Id}", i);
})
.WithTags("ToDo Items")
.WithDescription("יוצר משימה חדשה");

app.MapPut("/{id}", async (ToDoDbContext t, int id, Item updatedItem) =>
{
    var existingItem = await t.Items.FindAsync(id);
    if (existingItem == null) return Results.NotFound();

    if (updatedItem.Name != null)
        existingItem.Name = updatedItem.Name;
    existingItem.IsComplete = updatedItem.IsComplete;

    await t.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("ToDo Items")
.WithDescription("מעודכן משימה קיימת לפי מזהה");

app.MapDelete("/{id}", async (ToDoDbContext t, int id) =>
{
    var existingItem = await t.Items.FindAsync(id);
    if (existingItem == null) return Results.NotFound();

    t.Items.Remove(existingItem);
    await t.SaveChangesAsync();
    return Results.NoContent();
})
.WithTags("ToDo Items")
.WithDescription("מוחק משימה לפי מזהה");

app.MapGet("/status", () => "server API is running")
    .WithTags("System")
    .WithDescription("בודק אם השרת פעיל");

app.Run();
