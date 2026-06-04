using RestauranteApi.DataBase;
using RestauranteApi.Entities;
using RestauranteApi.Service.Interfaces;

namespace RestauranteApi.Service.Implementations
{
    public class ClientService : IClientService
    {
        private readonly RestauranteApiDbContext _RestauranteApiDbContext;

        public ClientService(RestauranteApiDbContext RestauranteApiDbContext)
        {
            _RestauranteApiDbContext = RestauranteApiDbContext;
        }

        public List<Client> GetAll()
        {
            return _RestauranteApiDbContext.Clients.ToList();
        }

        public Client? GetById(int id)
        {
            return _RestauranteApiDbContext.Clients.Find(id);
        }

        public Client Create(string name, string lastName, string phone)
        {
            Validate(name, lastName, phone);

            var client = new Client
            {
                Name = name,
                LastName = lastName,
                Phone = phone
            };
            _RestauranteApiDbContext.Clients.Add(client);
            _RestauranteApiDbContext.SaveChanges();
            return client;
        }

        public Client Update(int id, string name, string lastName, string phone)
        {
            Validate(name, lastName, phone, id);

            var client = _RestauranteApiDbContext.Clients.Find(id);
            if (client == null) throw new Exception($"Client with id {id} not found.");

            client.Name = name;
            client.LastName = lastName;
            client.Phone = phone;
            _RestauranteApiDbContext.SaveChanges();
            return client;
        }

        public void Delete(int id)
        {
            var client = _RestauranteApiDbContext.Clients.Find(id);
            if (client == null) throw new Exception($"Client with id {id} not found.");

            _RestauranteApiDbContext.Clients.Remove(client);
            _RestauranteApiDbContext.SaveChanges();
        }

        private void Validate(string name, string lastName, string phone, int? excludedClientId = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new Exception("El nombre del cliente es obligatorio.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new Exception("El apellido del cliente es obligatorio.");
            if (string.IsNullOrWhiteSpace(phone)) throw new Exception("El teléfono del cliente es obligatorio.");
            if (_RestauranteApiDbContext.Clients.Any(c => c.Phone == phone && c.Id != excludedClientId))
                throw new Exception($"Ya existe un cliente con el teléfono '{phone}'.");
        }
    }
}
