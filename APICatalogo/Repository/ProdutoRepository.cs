using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(APICatalogoDbContext context) : base(context) { }

        public PagedList<Produto> GetProdutos(ProdutosParameters produtosParameters)
        {
            //return Get()
            //     .OrderBy(p => p.Nome)
            //     .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
            //     .Take(produtosParameters.PageSize)
            //     .ToList();

            return PagedList<Produto>.ToPagedList(Get().OrderBy(p => p.ProdutoId),
                produtosParameters.PageNumber, produtosParameters.PageSize);
        }

        public IEnumerable<Produto> GetProdutosPorPreco()
        {
            return Get().OrderBy(p => p.Preco).ToList();
        }
    }
}