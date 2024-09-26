using System.ComponentModel.DataAnnotations;

namespace CoffeeShop.Models;

public class OrderDetailsModel
{
    [Required]
    public int OrderDetailID { get; set; }
    
    [Required]
    public int OrderID  { get; set; }
    
    [Required]
    public int ProductID  { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public double Amount  { get; set; }
    
    [Required]
    public double TotalAmount  { get; set; }
    
    [Required]
    public int UserID  { get; set; } 
}