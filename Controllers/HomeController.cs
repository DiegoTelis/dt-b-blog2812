using Blog.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController: ControllerBase
    {
        [HttpGet("")]
        [ApiKey] // USADO PARA USO DE ROBOS OU METODOS QUE ESTAO LIVRE PARA N USUARIOS S/ TOKEN
        public IActionResult Get(
            [FromServices] IConfiguration config)
        {
            var env = config.GetValue<string>("Env");
            return Ok(new
            {
                Environment = env,
                messagem = "Algo aqui"
            });
        }
            

    }
}
