using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.Categories
{
    public class EditorCategoryVM
    {
        [Required(ErrorMessage = "O Nome é Obrigatório")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Este campo deve ter entre 3 e 50 Caracteres")]
        public string Name { get; set; }
        [Required(ErrorMessage = "O Slug é Obrigatório")]
        public string Slug { get; set; }
    }
}
