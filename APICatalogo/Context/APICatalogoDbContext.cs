using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Context;

public class APICatalogoDbContext : DbContext
{
    public APICatalogoDbContext(DbContextOptions<APICatalogoDbContext> options) : base(options) { }


    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
}