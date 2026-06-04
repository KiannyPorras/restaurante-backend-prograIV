using System.ComponentModel.DataAnnotations;

namespace RestauranteApi.DTOs
{
    public class ClientCreateDto
    {
        [Required, StringLength(80)]
        public string Name { get; set; } = string.Empty;
        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required, StringLength(30)]
        public string Phone { get; set; } = string.Empty;
    }

    public class ClientResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }
}
