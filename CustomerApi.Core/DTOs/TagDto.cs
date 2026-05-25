using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Core.DTOs;

public class TagDto
{
    public int Id { get; set; }
    public string TagName { get; set; } = string.Empty;
}

public class CreateTagDto
{
    [Required]
    [MaxLength(100)]
    public string TagName { get; set; } = string.Empty;
}
