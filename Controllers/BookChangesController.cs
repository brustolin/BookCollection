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
public class BooksChangesController : ControllerBase
{
    private readonly BookCollectionDataContext _dataContext;
    private readonly EntityQueryComposer _queryComposer;

    public BooksChangesController(BookCollectionDataContext dataContext, EntityQueryComposer queryComposer)
    {
        _dataContext = dataContext;
        _queryComposer = queryComposer;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<BookChangeDTO>), StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
    public async Task<IActionResult> List([FromQuery] Filter[]? filters, [FromQuery] Sort[]? sort, int page = 1, int pageSize = 10)
    {
        var from = _dataContext.BookChanges.Include(b => b.Book);
        var query = _queryComposer.FilterAndSort(from, filters, sort);
        var response = await PaginatedResponse<BookChangeDTO>.PaginateQueryAndMapAsync(query, page, pageSize, p => new BookChangeDTO(p));
        return Ok(response);
    }
}