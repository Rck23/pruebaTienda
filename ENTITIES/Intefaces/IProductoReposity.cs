
using ENTITIES.Entities;

namespace ENTITIES.Intefaces;

public interface IProductoReposity : IGenericRepository<Producto>
{
    Task<IEnumerable<Producto>> GetProductosMasCaros(int cantidad);
}
