﻿using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {
        Task<PagedList<Categoria>> GetCategorias(CategoriasParameters categoriaParameters);
        Task<PagedList<Categoria>> GetCategoriasFiltroNome(CategoriasFiltroNome categoriaFiltroParams);
        Task<IEnumerable<Categoria>> GetCategoriasProdutos();
    }
}