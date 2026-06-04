namespace RestauranteApi.Entities
{
    public class WaitingList
    {
        public int Id { get; set; }

        public DateTime ReqDate { get; set; }

        public DateOnly DesiredDay { get; set; }

        public TimeSpan DesiredTime { get; set; }

        public int GuestCount { get; set; }

        public string PreferZone { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;
    }
}
