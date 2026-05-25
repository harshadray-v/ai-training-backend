using CustomerApi.Core.Data;
using CustomerApi.Core.DTOs;
using CustomerApi.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Api.Controllers;

[ApiController]
[Route("api/tags")]
public class TagsController : ControllerBase
{
    private readonly CustomerDbContext _context;

    public TagsController(CustomerDbContext context)
    {
        _context = context;
    }

    // GET /api/tags - list all tags
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tags = await _context.Tags
            .AsNoTracking()
            .OrderBy(t => t.TagName)
            .Select(t => new TagDto { Id = t.Id, TagName = t.TagName })
            .ToListAsync();

        return Ok(tags);
    }

    // POST /api/tags - create a new tag
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTagDto dto)
    {
        var exists = await _context.Tags.AnyAsync(t => t.TagName == dto.TagName);
        if (exists) return Conflict(new { message = $"Tag '{dto.TagName}' already exists." });

        var tag = new Tag { TagName = dto.TagName };
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), null, new TagDto { Id = tag.Id, TagName = tag.TagName });
    }

    // DELETE /api/tags/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tag = await _context.Tags.FindAsync(id);
        if (tag is null) return NotFound();

        _context.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

// Nested controller for customer-tag assignments
[ApiController]
[Route("api/customers/{customerId:int}/tags")]
public class CustomerTagsController : ControllerBase
{
    private readonly CustomerDbContext _context;

    public CustomerTagsController(CustomerDbContext context)
    {
        _context = context;
    }

    // GET /api/customers/{customerId}/tags
    [HttpGet]
    public async Task<IActionResult> GetTags(int customerId)
    {
        var customer = await _context.Customers
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == customerId);

        if (customer is null) return NotFound();

        var tags = customer.Tags.Select(t => new TagDto { Id = t.Id, TagName = t.TagName });
        return Ok(tags);
    }

    // POST /api/customers/{customerId}/tags/{tagId} - assign tag to customer
    [HttpPost("{tagId:int}")]
    public async Task<IActionResult> AssignTag(int customerId, int tagId)
    {
        var customer = await _context.Customers
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == customerId);

        if (customer is null) return NotFound();

        var tag = await _context.Tags.FindAsync(tagId);
        if (tag is null) return NotFound();

        if (customer.Tags.Any(t => t.Id == tagId))
            return Ok(new TagDto { Id = tag.Id, TagName = tag.TagName });

        customer.Tags.Add(tag);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTags), new { customerId }, new TagDto { Id = tag.Id, TagName = tag.TagName });
    }

    // DELETE /api/customers/{customerId}/tags/{tagId} - remove tag from customer
    [HttpDelete("{tagId:int}")]
    public async Task<IActionResult> RemoveTag(int customerId, int tagId)
    {
        var customer = await _context.Customers
            .Include(c => c.Tags)
            .FirstOrDefaultAsync(c => c.Id == customerId);

        if (customer is null) return NotFound();

        var tag = customer.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tag is null) return NotFound();

        customer.Tags.Remove(tag);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
