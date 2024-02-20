using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.Accounts;
public class UploadImageVM
{
    [Required(ErrorMessage ="Imagem Invalida")]
    public string Base64Image { get; set; }

}
