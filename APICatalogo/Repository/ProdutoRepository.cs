using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repository
{
    public class ProdutoRepository : Repository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(APICatalogoDbContext context) : base(context) { }

        public IEnumerable<Produto> GetProdutosPorPreco()
        {
           return Get().OrderBy(p => p.Preco).ToList();
        }
    }
}