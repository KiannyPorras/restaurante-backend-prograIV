using RestauranteApi.Entities;

namespace RestauranteApi.Service.Interfaces
{
    public interface IClientService
    {
        List<Client> GetAll();
        Client? GetById(int id);
        Client Create(string name, string lastName, string phone);
        Client Update(int id, string name, string lastName, string phone);
        void Delete(int id);
    }
}
