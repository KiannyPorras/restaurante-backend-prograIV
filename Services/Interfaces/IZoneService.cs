using RestauranteApi.Entities;

namespace RestauranteApi.Service.Interfaces
{
    public interface IZoneService
    {
        List<Zone> GetAll();
        Zone? GetById(int id);
        Zone Create(string name);
        Zone Update(int id, string name);
        void Delete(int id);
    }
}
