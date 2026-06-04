using System.ComponentModel.DataAnnotations;

namespace RestauranteApi.DTOs
{
    public class ZoneCreateDto
    {
        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;
    }

    public class ZoneResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
