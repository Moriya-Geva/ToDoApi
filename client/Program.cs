using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>(options => options.UseMySql
(builder.Configuration.GetConnectionString("ToDoDB"),
ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ToDoDB"))));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options => options.AddPolicy("Everything", policy =>
{
    policy
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin();
}));


var app = builder.Build();
app.UseCors("Everything");

app.MapGet("/", async (ToDoDbContext t) => 
{
    return await t.Items.ToListAsync();
});

app.MapPost("/", async(ToDoDbContext t, Item i) =>
{
    t.Items.Add(i);
    await t.SaveChangesAsync();

    return Results.Created($"/{i.Id}", i);
});

app.MapPut("/{id}", async (ToDoDbContext t, int id, Item updatedItem) =>
{
    var existingItem = await t.Items.FindAsync(id);
    if (existingItem == null)
    {
        return Results.NotFound();
    }

    // עדכון שדות הפריט
    if(updatedItem.Name!=null)
        existingItem.Name = updatedItem.Name;
    existingItem.IsComplete = updatedItem.IsComplete;
    

    await t.SaveChangesAsync();

    return Results.NoContent(); 

});


app.MapDelete("/{id}", async (ToDoDbContext t, int id) =>
{
    var existingItem = await t.Items.FindAsync(id);
    if (existingItem == null)
    {
        return Results.NotFound();
    }

    t.Items.Remove(existingItem);
    await t.SaveChangesAsync();

    return Results.NoContent(); 
});

app.UseCors("Everything"); 
app.Run();