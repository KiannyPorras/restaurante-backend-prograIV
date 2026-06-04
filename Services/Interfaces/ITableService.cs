using RestauranteApi.Entities;

namespace RestauranteApi.Service.Interfaces
{
    public interface ITableService
    {
        List<Table> GetAll();
        Table? GetById(int id);
        List<Table> GetBySectionId(int sectionId);
        List<Table> GetByZoneId(int zoneId);
        Table Create(string number, int capacity, int sectionId);
        Table Update(int id, string number, int capacity, int sectionId);
        void Activate(int id);
        void Deactivate(int id);
        void Delete(int id);
    }
}
