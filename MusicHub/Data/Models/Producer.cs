using System.ComponentModel.DataAnnotations;

namespace MusicHub.Data.Models;

public class Producer
{
    public Producer()
    {
        this.Albums = new HashSet<Album>();
    }
    public int Id { get; set; }

    [MaxLength(ValidationConstants.ProducerNameMaxLength)]
    public string Name { get; set; } = null!;

    [MaxLength(ValidationConstants.ProducerPseudonymMaxLength)]
    public string? Pseudonym { get; set; }

    [Phone]
    public string? PhoneNumber { get; set; }

    public virtual ICollection<Album> Albums { get; set; }
}
