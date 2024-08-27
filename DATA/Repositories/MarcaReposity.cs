
using DATA.Data;
using ENTITIES.Entities;
using ENTITIES.Intefaces;

namespace DATA.Repositories;

public class MarcaReposity : GenericRepository<Marca>, IMarcaReposity
{
    public MarcaReposity(TiendaContext context) : base(context)
    {

    }
}
