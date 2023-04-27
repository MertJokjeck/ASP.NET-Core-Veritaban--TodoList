using _1.Models.Entities;

using System.ComponentModel.DataAnnotations;

namespace _1.Models;

public class TodoViewModel
{
    [Required(ErrorMessage = "Lütfen Boş geçmeyiniz")]
    public string? Title { get; set; }

    [Required(ErrorMessage = "Lütfen Boş geçmeyiniz")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Lütfen Boş geçmeyiniz")]
    public string? Password { get; set; }
    public IEnumerable<Todo>? Todos { get; set; }
    public IEnumerable<User>? Users { get; set; }
}
