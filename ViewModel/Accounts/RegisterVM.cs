using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.Accounts;

public class RegisterVM
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    public string Name { get; set; }

    [Required(ErrorMessage = "E-mail é obrigatório")]
    [EmailAddress(ErrorMessage = "O E-mail é invalido")]
    public string Email { get; set; }
}
