using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;

namespace Customer.API.Entities;

public sealed class Customer : EntityBase<int>
{
    [Required]
    public string? Username { get; set; }
    
    [Required]
    [Column(TypeName = "VARCHAR(100)")]
    public string? FirstName { get; set; }
    
    [Required]
    [Column(TypeName = "VARCHAR(150)")]
    public string? LastName { get; set; }
    
    [Required]
    [EmailAddress]
    public string? EmailAddress { get; set; }
}