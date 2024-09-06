using Bogus;
using Boguser.API.Domain;
using Boguser.API.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Boguser.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(ApplicationDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Book>> CreateBook([FromBody] Book book)
    {
        if (book == null)
        {
            return BadRequest("Book data is required.");
        }

        context.Books.Add(book);

        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(Book), book);
    }


    [HttpGet]
    public async Task<ActionResult<List<Book>>> GetBookList([FromQuery] bool useMock = false)
    {
        if (!useMock)
        {
            var role = await context.Books
                .ToListAsync();

            if (role.Count > 0)
            {
                return Ok(role);
            }
        }

        var faker = new Faker<Book>()
            .RuleFor(r => r.Id, f => f.IndexFaker + 1)
            .RuleFor(r => r.Name, f => f.Name.FullName())
            .RuleFor(r => r.CreatedDate, f => f.Date.Past())
            .RuleFor(r => r.CreatedBy, f => f.Name.FullName());

        var fakeResponse = faker.Generate(20);

        return Ok(fakeResponse);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBookById(int id, [FromQuery] bool useMock = false)
    {
        if (!useMock)
        {
            var role = await context.Books
                .Where(r => r.Id == id)
                .FirstOrDefaultAsync();

            if (role != null)
            {
                return Ok(role);
            }
        }

        var faker = new Faker<Book>()
            .RuleFor(r => r.Id, id)
            .RuleFor(r => r.Name, f => f.Name.FullName())
            .RuleFor(r => r.CreatedDate, f => f.Date.Past())
            .RuleFor(r => r.CreatedBy, f => f.Name.FullName());

        var fakeResponse = faker.Generate();

        return Ok(fakeResponse);
    }
}