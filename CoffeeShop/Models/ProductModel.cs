using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Models;

public class ProductModel
{
    [HiddenInput]
    public int ProductID { get; set; }
    
    [Required(ErrorMessage = "Product Name is required")]
    public string ProductName  { get; set; }
    
    [Required]
    public double ProductPrice  { get; set; }
    
    [Required]
    [StringLength(5)]
    public string ProductCode { get; set; }
    
    [Required]
    public string Description  { get; set; }
    
    [Required]
    public int UserID { get; set; }
}

public class ProductDropDownModel
{
    public int ProductID { get; set; }
    public string ProductName { get; set; }
}