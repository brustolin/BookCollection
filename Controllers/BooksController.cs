using System.Net.Mime;
using BookStore.Models.DTO;
using BookTracker.Data;
using BookTracker.Models;
using BookTracker.Tools.QueryComposer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace BookTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly BookCollectionDataContext _dataContext;
    private readonly EntityQueryComposer _queryComposer;

    public BooksController(BookCollectionDataContext dataContext, EntityQueryComposer queryComposer)
    {
        _dataContext = dataContext;
        _queryComposer = queryComposer;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<BookDTO>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> List([FromQuery] Filter[]? filters, [FromQuery] Sort[]? sort, int page = 1, int pageSize = 10)
    {
        var from = _dataContext.Books.Include(b => b.Authors);
        var query = _queryComposer.FilterAndSort(from, filters, sort);
        var response = await PaginatedResponse<BookDTO>.PaginateQueryAndMapAsync(query, page, pageSize, p => new BookDTO(p));
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookDTO), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Details(int id)
    {
        var book = await _dataContext.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == id);
        return book == null ? NotFound() : Ok(new BookDTO(book));
    }

    [HttpPost]
    public async Task<IActionResult> Save([FromBody] BookDTO request)
    {
        if (request == null) { return BadRequest("Invalid book data."); }

        var book = request.Id == 0 ? _dataContext.Books.Add(new()).Entity : await _dataContext.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == request.Id);
        if (book == null) { return NotFound(); }

        var authorsIds = request.Authors.Where(a=>a.Id != 0).Select(a => a.Id).ToArray();
        var usedAuthors = await _dataContext.Authors.Where(a => authorsIds.Contains(a.Id)).ToListAsync();
        
        book.Title = request.Title;
        book.ShortDescription = request.ShortDescription;
        book.ReleaseDate = request.ReleaseDate;

        book.Authors.Clear();
        foreach (var authorDto in request.Authors) {
            var author = usedAuthors.FirstOrDefault(a => a.Id == authorDto.Id) ?? new Author { Name = authorDto.Name };
            book.Authors.Add(author);
        }   

        await _dataContext.SaveChangesAsync();
        return Ok(new BookDTO(book));
    }
}