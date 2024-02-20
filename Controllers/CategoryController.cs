using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModel;
using Blog.ViewModel.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync(
            [FromServices] IMemoryCache cache,
            [FromServices] BlogDataContext context)
        {
            //User.Identity
            try
            {
                var categories = cache.GetOrCreate("CategoriesCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetCategories(context);
                });



                return Ok(new ResultVM<List<Category>>(categories));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultVM<List<Category>>("05x06  - Falha interna no Servidor \n" + ex.Message + "\n" + ex.ToString));
            }
        }

        private List<Category> GetCategories(BlogDataContext context)
        {
            return context.Categories.ToList();
        }



        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultVM<Category>("Conteudo não Encontrado."));

                return Ok(new ResultVM<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultVM<Category>("05x07  - Não foi possivel incluir categoria"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResultVM<Category>("05x08  - Falha interna no Servidor"));
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryVM model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultVM<Category>(ModelState.GetErros()));

            try
            {
                var category = new Category
                {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug.ToLower()
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", new ResultVM<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultVM<Category>("05x09  - Não foi possivel incluir categoria"));
            }

            catch (Exception ex)
            {
                return StatusCode(500, new ResultVM<Category>("05x10  - Falha interna no Servidor\n" + ex.Message + "\n" + ex.ToString));
            }


        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryVM model,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    NotFound(new ResultVM<Category>("Conteúdo não encontrado!"));

                category.Name = model.Name;
                category.Slug = model.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(new ResultVM<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultVM<Category>("05x12  - Não foi possivel incluir categoria"));
            }

            catch
            {
                return StatusCode(500, new ResultVM<Category>("05x13  - Falha interna no Servidor"));
            }

        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    NotFound(new ResultVM<Category>("Conteúdo não encontrado"));

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new ResultVM<Category>(category));
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new ResultVM<Category>("05x14  - Não foi possivel incluir categoria"));
            }

            catch
            {
                return StatusCode(500, new ResultVM<Category>("05x15  - Falha interna no Servidor"));
            }

        }

    }
}
