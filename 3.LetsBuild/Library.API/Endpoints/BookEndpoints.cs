using FluentValidation.Results;
using Library.API.Models;
using Library.API.Services;
using Library.API.Validators;
using Microsoft.AspNetCore.Authorization;

namespace Library.API.Endpoints
{
    public static class BookEndpoints
    {
        public static void UseBookEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("login", (JwtProvider jwtProvider) =>
            {
                return Results.Ok(new { Token = jwtProvider.CreateToken() });
            }).WithTags("Auth");

            app.MapPost("books", async (Book book, IBookService service, CancellationToken cancellationToken) =>
            {
                BookValidator validator = new();
                ValidationResult validationResult = validator.Validate(book);
                if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors.Select(s => s.ErrorMessage));

                var result = await service.CreateAsync(book, cancellationToken);
                if (!result) return Results.BadRequest("Something went wrong!");

                //return Results.Created($"/books/{book.Isbn}", book);
                return Results.CreatedAtRoute("GetBook", new { isbn = book.Isbn });
            }).WithTags("Books");


            app.MapGet("books", [Authorize] async (IBookService service, CancellationToken cancellationToken) =>
            {
                var books = await service.GetAllAsync(cancellationToken);
                return Results.Ok(books);
            }).WithTags("Books");

            app.MapGet("books/{isbn}", async (string isbn, IBookService service, CancellationToken cancellationToken) =>
            {
                Book? book = await service.GetByIsbnAsync(isbn, cancellationToken);
                return Results.Ok(book);
            }).WithName("GetBook")
            .WithTags("Books");

            app.MapGet("get-books-by-title/{title}", async (string title, IBookService service, CancellationToken cancellationToken) =>
            {
                var books = await service.SearchByTitleAsync(title, cancellationToken);
                return Results.Ok(books);
            }).WithTags("Books");

            app.MapPut("books", async (Book book, IBookService service, CancellationToken cancellationToken) =>
            {
                BookValidator validator = new();
                ValidationResult validationResult = validator.Validate(book);
                if (!validationResult.IsValid) return Results.BadRequest(validationResult.Errors.Select(s => s.ErrorMessage));

                var result = await service.UpdateAsync(book, cancellationToken);
                if (!result) return Results.BadRequest("Something went wrong!");

                return Results.Ok(new { Message = "Book update is successful" });
            }).WithTags("Books");

            app.MapDelete("books/{isbn}", async (string isbn, IBookService service, CancellationToken cancellationToken) =>
            {
                var result = await service.DeleteAsync(isbn, cancellationToken);
                if (!result) return Results.BadRequest("Something went wrong!");

                return Results.Ok(new { Message = "Book delete is successful" });
            }).WithTags("Books");
        }
    }
}
