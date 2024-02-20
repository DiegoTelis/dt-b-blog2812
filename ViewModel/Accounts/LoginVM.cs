using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.Accounts;

public class LoginVM
{
    [Required(ErrorMessage = "Email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email invalido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Informe a senha")]
    public string Password { get; set; }

}
