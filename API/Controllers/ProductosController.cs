﻿using API.Dtos;
using API.Helpers;
using API.Helpers.Errors;
using AutoMapper;
using ENTITIES.Entities;
using ENTITIES.Intefaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]


[Authorize(Roles = "Administrador")]
public class ProductosController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductosController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpGet]

    public async Task<ActionResult<Pager<ProductoListDto>>> Get([FromQuery] Params productParams)
    {
        var resultado = await _unitOfWork.Productos
                                    .GetAllAsync(productParams.PageIndex, productParams.PageSize,
                                    productParams.Search);

        var listaProductosDto = _mapper.Map<List<ProductoListDto>>(resultado.registros);

        Response.Headers.Add("X-InlineCount", resultado.totalRegistros.ToString());

        return new Pager<ProductoListDto>(listaProductosDto, resultado.totalRegistros,
            productParams.PageIndex, productParams.PageSize, productParams.Search);

    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductoDto>>> Get11()
    {
        var productos = await _unitOfWork.Productos.GetAllAsync();


        return _mapper.Map<List<ProductoDto>>(productos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductoDto>> Get(int id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);

        if (producto == null)
        {
            return NotFound(new ApiResponse(404, "El producto solicitado no existe."));
        }
        return _mapper.Map<ProductoDto>(producto);
    }

    // POST: api/Productos
    [HttpPost]
    public async Task<ActionResult<Producto>> Post(ProductoAddUpdateDto productoDto)
    {
        var producto = _mapper.Map<Producto>(productoDto);

        _unitOfWork.Productos.Add(producto);
        await _unitOfWork.SaveAsync();

        if (producto == null)
        {
            return BadRequest(new ApiResponse(400));
        }

        productoDto.Id = producto.Id;
        return CreatedAtAction(nameof(Post), new { id = productoDto.Id }, productoDto);
    }

    //PUT: api/Productos/id
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductoAddUpdateDto>> Put(int id, [FromBody] ProductoAddUpdateDto productoDto)
    {
        if (productoDto == null)
        {
            return NotFound(new ApiResponse(404, "El producto solicitado no existe."));
        }

        var productoBd = await _unitOfWork.Productos.GetByIdAsync(id);

        if (productoBd == null)
        {
            return NotFound(new ApiResponse(404, "El producto solicitado no existe."));
        }

        var producto = _mapper.Map<Producto>(productoDto);

        _unitOfWork.Productos.Update(producto);
        await _unitOfWork.SaveAsync();

        return productoDto;
    }

    //DELETE: api/productos/id 
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var producto = await _unitOfWork.Productos.GetByIdAsync(id);

        if (producto == null)
        {
            return NotFound(new ApiResponse(404, "El producto solicitado no existe."));
        }

        _unitOfWork.Productos.Remove(producto);
        await _unitOfWork.SaveAsync();

        return NoContent();
    }
}

