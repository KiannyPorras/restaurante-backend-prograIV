namespace RestauranteApi.Entities
{
    public class Section
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int ZoneId { get; set; }

        public Zone Zone { get; set; } = null!;

        public ICollection<Table> Tables { get; set; } = new List<Table>();
    }
}
