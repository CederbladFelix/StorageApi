using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageApi.Data;
using StorageApi.DTOs;
using StorageApi.Models;

namespace StorageApi.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly StorageContext _context;

        public ProductsController(StorageContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts(
                                                                        [FromQuery] string? category,
                                                                        [FromQuery] string? name
                                                                        )
        {
            var productsQuery = _context.Products.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(category))           
                productsQuery = productsQuery.Where(product => product.Category == category);

            if (!string.IsNullOrWhiteSpace(name))
                productsQuery = productsQuery.Where(product => product.Name.ToLower().Contains(name.ToLower()));



            var products = await productsQuery.ToListAsync();

            var productsDtos = products
                                .Select(product => new ProductDto
                                {
                                    Id = product.Id,
                                    Name = product.Name,
                                    Price = product.Price,
                                    Count = product.Count
                                })
                                .ToList();

            return Ok(productsDtos);

        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Count = product.Count
            };

            return Ok(productDto);
        }

        // GET: api/Products/stats
        [HttpGet("stats")]
        public async Task<ActionResult<StatsDto>> GetStats()
        {
            var products = await _context.Products.ToListAsync();

            if (!products.Any())
                return Ok(new StatsDto());

            var statsDto = new StatsDto
            {
                TotalNumberOfProducts = products.Count,
                TotalInventoryValue = products.Sum(product => product.InventoryValue),
                AveragePrice = Math.Round(products.Average(product => product.Price), 2)
            };


            return Ok(statsDto);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, CreateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Name = dto.Name;
            product.Price = dto.Price;
            product.Category = dto.Category;
            product.Shelf = dto.Shelf;
            product.Count = dto.Count;
            product.Description = dto.Description;
              
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ProductDto>> PostProduct(CreateProductDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Category = dto.Category,
                Shelf = dto.Shelf,
                Count = dto.Count,
                Description = dto.Description
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = dto.Name,
                Price = dto.Price,
                Count = dto.Count
            };

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }

        // DELETE: api/Products/5
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

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
