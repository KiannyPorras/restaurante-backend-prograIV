using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;
namespace RestauranteApi.Service.Interfaces
{
    public interface IReservationService
    {
        public List<Reservation> GetAll();
        public Reservation? GetById(int id);
        public List<Reservation> GetByClientId(int clientId);
        public Reservation Create(int clientId, int tableId, DateOnly date, TimeSpan reservationTime, int guestCount);
        public Reservation Update(int id, DateOnly date, TimeSpan reservationTime, int guestCount);
        public void Cancel(int id);
        public void MarkAsAttended(int id);
        public List<ReservationHistory> GetHistory(int reservationId);
    }
}
