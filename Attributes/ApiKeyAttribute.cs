using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // a Query seria a passada na requisição em http://localhost:51001?api_key=CHAVE - seria o api_key
        //vai tentar pegar o valor da variavel ApiKeyName, conseguindo gera uma variavel : extractedApiKey
        if (!context.HttpContext.Request.Query.TryGetValue(Configuration.ApiKeyName, out var extractedApiKey)) 
        {
            context.Result = new ContentResult()
            {
                StatusCode = 401,
                Content =  "ApiKey não encontrada"
            };
            return;
        }

        //verifica se a variavel é igual a gerada no out do metodo acima
        if(!Configuration.ApiKey.Equals(extractedApiKey))
        {
            context.Result = new ContentResult()
            {
                StatusCode = 403,
                Content = "Acesso não Autorizado"
            };
            return;
        }
        //se autorizado,  passa normal
        await next();

    }
}
