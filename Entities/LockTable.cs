namespace RestauranteApi.Entities
{
    public class LockTable
    {
        public int Id { get; set; }

        public string Reason { get; set; } = string.Empty;

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public bool IsActive { get; set; } 

        public int TableId { get; set; }

        public Table Table { get; set; } = null!;
    }
}
