using RestauranteApi.Entities;

namespace RestauranteApi.Service.Interfaces
{
    public interface IWaitingListService
    {
        List<WaitingList> GetAll();
        WaitingList? GetById(int id);
        List<WaitingList> GetPending();
        WaitingList Create(int clientId, DateOnly desiredDay, TimeSpan desiredTime, int guestCount, string? preferZone = null);
        Reservation AssignTable(int waitingListId, int tableId);
        void Cancel(int id);
    }
}
