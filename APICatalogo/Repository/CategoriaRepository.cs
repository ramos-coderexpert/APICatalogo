using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repository
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository(APICatalogoDbContext context) : base(context) { }


        public PagedList<Categoria> GetCategorias(CategoriasParameters categoriaParameters)
        {
            return PagedList<Categoria>.ToPagedList(Get().OrderBy(c => c.CategoriaId),
                 categoriaParameters.PageNumber, categoriaParameters.PageSize);
        }

        public IEnumerable<Categoria> GetCategoriasProdutos()
        {
            return Get().Include(c => c.Produtos);
        }        
    }
}