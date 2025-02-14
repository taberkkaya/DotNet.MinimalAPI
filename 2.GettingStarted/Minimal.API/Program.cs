using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Minimal.API;

var builder = WebApplication.CreateBuilder(args);

//service registration starts here

builder.Services.AddScoped<PeopleService>();
builder.Services.AddScoped<GuidGenerator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

//service registration stops here

var app = builder.Build();

//Middleware registration starts here

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("get-example",() => "Hello from GET");
app.MapPost("post-example",() => "Hello from POST");

app.MapGet("okey-object", () => Results.Ok(new {Message = "API is working..."}));

app.MapGet("slow-request", async () => {
    await Task.Delay(2000);

    return Results.Ok(new { Message = "Slow API is working..."});
});

app.MapGet("get", () => "This is a GET");
app.MapPost("post", () => "This is a POST");
app.MapPut("put", () => "This is a PUT");
app.MapDelete("delete", () => "This is a DELETE");

app.MapMethods("options-or-head", 
    new[] {"HEAD","OPTIONS"},
    () => "Hello from either options or head");

var handler = () => "This is coming from a var";

app.MapGet("handler", handler);

app.MapGet("from-class", Example.SomeMethod);

app.MapGet("get-params/{age:int}", (int age) =>
{
    return $"Age provided was {age}";
});

app.MapGet("cars/{carId:regex(^[a-z0-9+$])}", (string carId) =>
{
    return $"Card ID provided was : {carId}";
});

app.MapGet("books/{isbn:length(11)}", (string isbn) =>
{
    return $"Isbn provided was: {isbn}";
});

app.MapGet("people/search", (string? searchTerm, PeopleService peopleService) => 
{ 
    if(searchTerm is null) return Results.NotFound();

    var result = peopleService.Search(searchTerm);
    return Results.Ok(result);
    
});

app.MapGet("mix/{routeParams}", (
    [FromRoute] string routeParams, 
    [FromQuery(Name = "q")] int queryParams, 
    [FromServices] GuidGenerator guidGenerator) => 
{
    return $"{routeParams} {queryParams} {guidGenerator.NewGuid}";
});

app.MapPost("people", (Person person) =>
{
    return Results.Ok(person);
});

app.MapGet("httpContext", async context =>
{
    await context.Response.WriteAsync("Hello from the httpContext");
});

app.MapGet("http", async (HttpRequest request, HttpResponse response) =>
{
    var queryString = request.QueryString;
    await response.WriteAsync($"Hello from the http, query string is : {queryString}");
});

app.MapGet("claims", (ClaimsPrincipal user) =>
{
    var claims = user.Claims.ToList();
    return Results.Ok(claims);
});

app.MapGet("cancel", (CancellationToken cancellationToken) =>
{
    return Results.Ok();
});

app.MapGet("get-point", (MapPoint point) => //localhost:7101?point=10.55,1.555);
{
    return Results.Ok(point);
});


app.MapPost("post-point", (MapPoint point) =>
{
    return Results.Ok(point);
});

app.MapGet("simple-string", () => "Hello World");

app.MapGet("json-raw-obj", () => new {Message = "Hello World"});

app.MapGet("ok-obj", () => Results.Ok(new { Message = "Hello World" }));

app.MapGet("json-obj", () => Results.Json(new { Message = "Hello World" }));

app.MapGet("text-string", () => Results.Text("Hello World"));

app.MapGet("redirect", () => Results.Redirect("https://google.com"));

app.MapGet("download", () => Results.File("./myfile.txt"));

app.MapGet("logging", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello from endpoint");
    return Results.Ok();
});

app.MapControllers();

//Middleware registration stops here

app.Run();
