using Minimal.API;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//app.Urls.Add("https://localhost:7777");
//app.Urls.Add("http://localhost:8888");

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

app.Run();
