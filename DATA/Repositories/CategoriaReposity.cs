using DATA.Data;
using ENTITIES.Entities;
using ENTITIES.Intefaces;


namespace DATA.Repositories;

public class CategoriaReposity : GenericRepository<Categoria>, ICategoriaReposity
{
    public CategoriaReposity(TiendaContext context) : base(context)
    {

    }
}
