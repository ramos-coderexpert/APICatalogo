﻿using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(APICatalogoDbContext context) : base(context) { }


        public async Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriaParameters)
        {
            return await PagedList<Categoria>.ToPagedList(Get().OrderBy(c => c.CategoriaId),
                 categoriaParameters.PageNumber, categoriaParameters.PageSize);
        }

        public async Task<PagedList<Categoria>> GetCategoriasFiltroNome(CategoriasFiltroNome categoriaFiltroParams)
        {
            var categorias = Get().AsQueryable();

            if (!string.IsNullOrEmpty(categoriaFiltroParams.Nome))
                categorias = categorias.Where(c => c.Nome.Contains(categoriaFiltroParams.Nome));

            var categoriasFiltradas = await PagedList<Categoria>.ToPagedList(categorias, categoriaFiltroParams.PageNumber, categoriaFiltroParams.PageSize);
            return categoriasFiltradas;
        }

        public async Task<IEnumerable<Categoria>> GetCategoriasProdutos()
        {
            return await Get().Include(c => c.Produtos).ToListAsync();
        }
    }
}