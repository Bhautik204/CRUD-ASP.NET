using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeShop.Models;

public class OrderModel
{
    [HiddenInput]
    public int OrderID { get; set; }
    
    [Required]
    public DateTime OrderDate  { get; set; }
    
    [Required]
    public int CustomerID  { get; set; }
    
    [Required]
    public string PaymentMode { get; set; }
    
    [Required]
    public double? TotalAmount  { get; set; }
    
    [Required]
    public string ShippingAddress { get; set; }
    
    [Required]
    public int UserID  { get; set; }
}

public class OrderDropDownModel
{
    public int OrderID { get; set; }
    public DateTime OrderDate { get; set; }
}
