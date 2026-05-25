using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerApi.Core.Entities;

public class Tag
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string TagName { get; set; } = string.Empty;

    // Navigation property (many-to-many)
    public ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
