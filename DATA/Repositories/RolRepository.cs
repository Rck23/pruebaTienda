using DATA.Data;
using ENTITIES.Entities;
using ENTITIES.Intefaces;

namespace DATA.Repositories;

public class RolRepository : GenericRepository<Rol>, IRolRepository
{
    public RolRepository(TiendaContext context) : base(context)
    {
    }
}

