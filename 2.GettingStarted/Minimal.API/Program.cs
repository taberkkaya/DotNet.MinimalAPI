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

app.Run();
