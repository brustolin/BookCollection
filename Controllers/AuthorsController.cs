using BookStore.Models.DTO;
using BookTracker.Data;
using BookTracker.Models;
using BookTracker.Tools.QueryComposer;
using Microsoft.AspNetCore.Mvc;

namespace BookTracker.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly BookCollectionDataContext _dataContext;

    public AuthorsController(BookCollectionDataContext dataContext)
    {
        _dataContext = dataContext;
    }

    [HttpGet]
    public async Task<IActionResult> List(string? name,int page = 1, int pageSize = 10)
    {
        IQueryable<Author> query = _dataContext.Authors.OrderBy(a => a.Name);
        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(a => a.Name.ToLower().Contains(name.ToLower()));
        }

        var response = await PaginatedResponse<AuthorDTO>.PaginateQueryAndMapAsync(query, page, pageSize, a => new AuthorDTO(a));
        return Ok(response);
    }
}