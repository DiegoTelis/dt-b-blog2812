using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Blog.Extensions
{
    public static class ModelStateExtention
    {
        
        public static List<string> GetErros(this ModelStateDictionary modelState)
        {
            List<string> result = new ();
            foreach (var item in modelState.Values)
            {
                result.AddRange(item.Errors.Select(x => x.ErrorMessage));
            }

            return result;

        }

    }
}
