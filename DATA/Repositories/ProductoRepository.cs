﻿using DATA.Data;
using ENTITIES.Entities;
using ENTITIES.Intefaces;
using Microsoft.EntityFrameworkCore;

namespace DATA.Repositories;

public class ProductoRepository : GenericRepository<Producto>, IProductoReposity
{
    public ProductoRepository(TiendaContext context) : base(context)
    {

    }


    public async Task<IEnumerable<Producto>> GetProductosMasCaros(int cantidad) =>

        await _context.Productos
                  .OrderByDescending(p => p.Precio).Take(cantidad).ToListAsync();

    public override async Task<Producto> GetByIdAsync(int id)
    {
        return await _context.Productos
            .Include(p => p.Marca)
            .Include(p => p.Categoria)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    // SOBREESCRIBIENDO EL METODO DE GENERIC REPOSITORY
    public override async Task<IEnumerable<Producto>> GetAllAsync()
    {
        return await _context.Productos
            .Include(u => u.Marca)
            .Include(u => u.Categoria)
            .ToListAsync();
    }

    public override async Task<(int totalRegistros, IEnumerable<Producto> registros)> GetAllAsync(int pageIndex, 
        int pageSize, string search)
    {
        var consulta = _context.Productos as IQueryable<Producto>;

        if(!String.IsNullOrEmpty(search))
        {
            consulta = consulta.Where(p=> p.Nombre.ToLower().Contains(search));
        }

        var totalRegistros = await consulta
                                    .CountAsync();

        var registros = await consulta
                                .Include(u => u.Marca)
                                .Include(u => u.Categoria)
                                .Skip((pageIndex - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return (totalRegistros, registros);
    }


}
