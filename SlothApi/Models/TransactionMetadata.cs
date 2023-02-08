using System.ComponentModel.DataAnnotations;

namespace SlothApi.Models;

public class TransactionMetadata
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string TransactionId { get; set; } = "";

    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = "";

    [Required]
    public string Value { get; set; } = "";
}
