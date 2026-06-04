using System.ComponentModel.DataAnnotations;

namespace RestauranteApi.DTOs
{
    public class TurnCreateDto
    {
        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
    }

    public class TurnResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsActive { get; set; }
    }
}
