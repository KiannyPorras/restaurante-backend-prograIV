using System.ComponentModel.DataAnnotations;

namespace RestauranteApi.DTOs
{
    public class SectionCreateDto
    {
        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;
        [Range(1, int.MaxValue)]
        public int ZoneId { get; set; }
    }

    public class SectionResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ZoneId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
    }
}
