using firstcrud.Controllers.Data;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace firstcrud.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(ProductDbContext context, ILogger<ProductController> logger) : ControllerBase
{
    private readonly ProductDbContext _context = context;

    /* Old way:
     public ProductController(ProductDbContext context)
    {
        _context = context;
    } 
    */ 
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
        var products = await _context.Products
            .Include(g => g.ProductDetails)
            .Include(g => g.Publisher)
            .Include(g => g.Customers)
            .ToListAsync();
        logger.LogInformation("Retrieved {productCount} products",  products.Count);
        return Ok(products);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProductById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        logger.LogInformation("Retrieved {productId} product",  id);
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> PostProduct(Product product) // todo: remake with PostProductDto
    {
        if (product == null)
        {
            return BadRequest();
        }
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutProduct(int id, Product updatedProduct)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        
        product.Name = updatedProduct.Name;
        product.Description = updatedProduct.Description;
        product.Price = updatedProduct.Price;
        product.Publisher = updatedProduct.Publisher;
        
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }

}