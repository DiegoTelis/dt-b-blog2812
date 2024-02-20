using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Service;
using Blog.ViewModel;
using Blog.ViewModel.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Text.RegularExpressions;

namespace Blog.Controllers;


[ApiController]
public class AccountController: ControllerBase
{
    [HttpPost("v1/accounts")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterVM model,
        [FromServices] BlogDataContext context,
        [FromServices] EmailService emailService)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultVM<string>(ModelState.GetErros()));

        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace('@', '-').Replace('.', '-')
        };

        var password = PasswordGenerator.Generate(25);

        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            emailService.Send(user.Name, user.Email,"Cadastro Realizado", $"sua senha é: <strong>{password} </strong>" );

            return Ok(new ResultVM<dynamic>( new
            {
                user = user.Email, password 
            }));

        }

        catch(DbUpdateException )
        {
            return StatusCode(400, new ResultVM<string>("05x99 - Este email já esta cadastrado"));
        }
        catch
        {
            return StatusCode(500, new ResultVM<string>("05x100 - Falha interna no servidor"));

        }


    }



    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginVM model,
        [FromServices] BlogDataContext context,
        [FromServices] TokenService tokenService
        )
    {
        if (!ModelState.IsValid)
            return BadRequest(new ResultVM<string>(ModelState.GetErros()));

        var user = await context.Users.AsNoTracking()
                                        .Include(x => x.Roles)
                                        .FirstOrDefaultAsync(x => x.Email == model.Email);
        
        //Verifica se não existe, e omite caso não
        if (user == null)
            return StatusCode(401, new ResultVM<string>("Usuario ou senha invalido"));

        if(!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultVM<string>("Usuario ou senha invalido"));

        try
        {
            var token = tokenService.GenereteToken(user);
            
            return Ok(new ResultVM<string>(token,null));
        }
        catch
        {
            return StatusCode(500, new ResultVM<string>("05X04 Falha interna no servidor"));
        }

        


    }


    [Authorize]
    [HttpPost("v1/accounts/uploadimage")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageVM model,
        [FromServices] BlogDataContext context)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Email == User.Identity.Name);
        if (user == null)
            return NotFound(new ResultVM<User>("05X06 Usuario não encontrado"));


        var fileName = $"{user.Slug}-{DateTime.UtcNow.ToString("s")}.jpg";
        fileName = fileName.Replace(":", "");

        var data = new Regex(@"^data:image\/[a-z]+;Base64,").Replace(model.Base64Image, "");
        var bytes = Convert.FromBase64String(data);

        try
        {
            System.IO.File.WriteAllBytesAsync($"wwwroot /images/{fileName}", bytes);
        }
        catch  
        {
            return StatusCode(500, new ResultVM<string>("05X05 Falha interna no servidor"));
        }

        

        

        user.Image = $"http://localhost:5011/images/{fileName}";
        try
        {
            context.Users.Update(user);
            context.SaveChanges();
        }
        catch
        {
            return StatusCode(500, new ResultVM<string>("05X06 Falha interna no servidor"));
        }

        return Ok(new ResultVM<string>("Imagem alterado com Sucesso!", null));

    }

} 
