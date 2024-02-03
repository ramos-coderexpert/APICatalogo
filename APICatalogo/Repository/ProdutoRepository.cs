﻿using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(APICatalogoDbContext context) : base(context) { }

        public async Task<PagedList<Produto>> GetProdutos(ProdutosParameters produtosParameters)
        {
            //return Get()
            //     .OrderBy(p => p.Nome)
            //     .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
            //     .Take(produtosParameters.PageSize)
            //     .ToList();

            var produtosOrdenados = Get().OrderBy(p => p.ProdutoId).AsQueryable();
            var produtosPaginados = await PagedList<Produto>.ToPagedList(produtosOrdenados, produtosParameters.PageNumber, produtosParameters.PageSize);
            return produtosPaginados;
        }

        public async Task<IEnumerable<Produto>> GetProdutosPorPreco()
        {
            return await Get().OrderBy(p => p.Preco).ToListAsync();
        }
    }
}