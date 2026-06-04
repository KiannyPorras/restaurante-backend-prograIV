using RestauranteApi.Entities;

namespace RestauranteApi.Service.Interfaces
{
    public interface ITurnService
    {
        List<Turn> GetAll();
        Turn? GetById(int id);
        Turn Create(string name, TimeSpan startTime, TimeSpan endTime);
        Turn Update(int id, string name, TimeSpan startTime, TimeSpan endTime);
        void Delete(int id);
        void Activate(int id);
        void Deactivate(int id);
        Turn? FindActiveTurnForTime(TimeSpan time);
    }
}
