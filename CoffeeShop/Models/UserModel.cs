using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Models;

public class UserModel
{
    [HiddenInput]
    public int UserID { get; set; }
    
    [Required]
    public string UserName  { get; set; }
    
    [Required]
    public string Email  { get; set; }
    
    [Required]
    public string Password { get; set; }
    
    [Required]
    [StringLength(10)]
    public string MobileNo  { get; set; }
    
    [Required]
    public string Address  { get; set; }
    
    [Required]
    public bool? IsActive  { get; set; }
    
}

public class UserDropDownModel
{
    public int UserID { get; set; }
    public string UserName { get; set; }
}

public class UserLoginModel
{
    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}

public class UserRegisterModel
{
    public int? UserID { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Mobile Number is required.")]
    public string MobileNo { get; set; }

    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }
}