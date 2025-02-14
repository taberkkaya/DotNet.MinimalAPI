using FluentValidation.Results;
using Library.API.Context;
using Library.API.Models;
using Library.API.Services;
using Library.API.Validators;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("MyDb");
});

builder.Services.AddScoped<IBookService, BookService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("books", async (Book book, IBookService service, CancellationToken cancellationToken) =>
{
    BookValidator validator = new();
    ValidationResult validationResult = validator.Validate(book);
    if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors.Select(s => s.ErrorMessage));

    var result = await service.CreateAsync(book, cancellationToken);
    if (!result) return Results.BadRequest("Something went wrong!");

    return Results.Ok(new {Message = "Book creation successful" });
});


app.MapGet("books", async (IBookService service, CancellationToken cancellationToken) =>
{
    var books = service.GetAllAsync(cancellationToken);
    return Results.Ok(books);
});

app.Run();
