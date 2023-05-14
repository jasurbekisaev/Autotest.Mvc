using System.ComponentModel.DataAnnotations;

namespace Autotest.Mvc.Models;

public class SignInUserModel
{
    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }
}