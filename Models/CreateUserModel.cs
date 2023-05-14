using System.ComponentModel.DataAnnotations;

namespace Autotest.Mvc.Models;

public class CreateUserModel
{
    [Required(ErrorMessage = "Username kiritilishi kerak")]
    // [StringLength(10, MinimumLength = 5)]
    public string? Username { get; set; }
    [Required]
    /*
    [RegularExpression("^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{8,}$",
        ErrorMessage = "Password must be minimum eight characters, at least one letter and one number")]*/
    public string? Password { get; set; }

    [Required]
    /* [MaxLength(20)]
     [MinLength(5)]*/
    public string? Name { get; set; }

    [Required]
    public IFormFile? Photo { get; set; }
}