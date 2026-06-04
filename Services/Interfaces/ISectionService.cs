using RestauranteApi.Entities;

namespace RestauranteApi.Service.Interfaces
{
    public interface ISectionService
    {
        List<Section> GetAll();
        Section? GetById(int id);
        List<Section> GetByZoneId(int zoneId);
        Section Create(string name, int zoneId);
        Section Update(int id, string name, int zoneId);
        void Delete(int id);
    }
}
