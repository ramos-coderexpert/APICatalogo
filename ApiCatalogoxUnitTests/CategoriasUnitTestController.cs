using APICatalogo.Context;
using APICatalogo.Controllers;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Repository;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiCatalogoxUnitTests
{
    public class CategoriasUnitTestController
    {
        private IMapper _mapper;
        private IUnitOfWork _repository;

        public static DbContextOptions<APICatalogoDbContext> dbContextOptions { get; }

        public static string connectionString =
           "Server=localhost\\SQLEXPRESS; Database=APICatalogoDB; Integrated Security=True; trustServerCertificate=true";

        static CategoriasUnitTestController()
        {
            dbContextOptions = new DbContextOptionsBuilder<APICatalogoDbContext>().UseSqlServer(connectionString).Options;
        }

        public CategoriasUnitTestController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            _mapper = config.CreateMapper();

            var context = new APICatalogoDbContext(dbContextOptions);

            //DBUnitTestsMockInitializer db = new DBUnitTestsMockInitializer();
            //db.Seed(context);

            _repository = new UnitOfWork(context);
        }

        //=======================================================================
        // Testes Unitários
        // Inicio dos testes : método GET

        [Fact]
        public async void GetCategorias_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(_repository, _mapper);

            //Act  
            var okResult = await controller.GetCategoriasProdutos();
            var data = okResult.Result.Should().BeAssignableTo<OkObjectResult>().Subject;

            //Assert  
            Assert.IsType<List<CategoriaDTO>>(data.Value);
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public async void GetCategorias_Return_BadRequestResult()
        {
            //Arrange  
            var controller = new CategoriasController(_repository, _mapper);

            //Act  
            var data = await controller.GetCategoriasProdutos();

            //Assert  
            Assert.IsType<BadRequestResult>(data.Result);
        }

        //GET retorna uma lista de objetos categoria
        //objetivo verificar se os dados esperados estão corretos
        [Fact]
        public async void GetCategorias_MatchResult()
        {
            //Arrange  
            var controller = new CategoriasController(_repository, _mapper);

            //Act  
            var data = await controller.GetCategoriasProdutos();

            //Assert  
            Assert.IsType<List<CategoriaDTO>>(data.Value);
            var cat = data.Value.Should().BeAssignableTo<List<CategoriaDTO>>().Subject;

            Assert.Equal("Bebidas", cat[0].Nome);
            Assert.Equal("bebidas.jpg", cat[0].ImagemUrl);

            Assert.Equal("Sobremesas", cat[2].Nome);
            Assert.Equal("sobremesas.jpg", cat[2].ImagemUrl);
        }

        //-------------------------------------------------------------
        //GET - Retorna categoria por Id : Retorna objeto CategoriaDTO
        [Fact]
        public async void GetCategoriaById_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(_repository, _mapper);
            var catId = 2;

            //Act  
            var okResult = await controller.GetCategoria(catId);
            var data = okResult.Result.Should().BeAssignableTo<OkObjectResult>().Subject;

            //Assert  
            Assert.IsType<OkObjectResult>(okResult.Result);
            Assert.IsType<CategoriaDTO>(data.Value);
        }

        //-------------------------------------------------------------
        //GET - Retorna Categoria por Id : NotFound
        [Fact]
        public async void GetCategoriaById_Return_NotFoundResult()
        {
            //Arrange  
            var controller = new CategoriasController(_repository, _mapper);
            var catId = 9999;

            //Act  
            var data = await controller.GetCategoria(catId);

            //Assert  
            Assert.IsType<NotFoundObjectResult>(data.Result);
        }

        //-------------------------------------------------------------
        // POST - Incluir nova categoria - Obter CreatedResult
        [Fact]
        public async void Post_Categoria_AddValidData_Return_CreatedResult()
        {
            //Arrange  
            var controller = new CategoriasController(_repository, _mapper);

            var cat = new CategoriaDTO() { Nome = "Teste Unitario Inclusao", ImagemUrl = "testecatInclusao.jpg" };

            //Act  
            var data = await controller.Post(cat);

            //Assert  
            Assert.IsType<CreatedAtRouteResult>(data);
        }

        //-------------------------------------------------------------
        //PUT - Atualizar uma categoria existente com sucesso
        [Fact]
        public async void Put_Categoria_Update_ValidData_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(_repository, _mapper);
            var catId = 3;

            //Act  
            var existingPost = await controller.GetCategoria(catId);
            var okResult = existingPost.Result.Should().BeAssignableTo<OkObjectResult>().Subject;
            var result = okResult.Value.Should().BeAssignableTo<CategoriaDTO>().Subject;

            var catDto = new CategoriaDTO();
            catDto.CategoriaId = catId;
            catDto.Nome = "Categoria Atualizada - Testes 1";
            catDto.ImagemUrl = result.ImagemUrl;

            var updatedData = await controller.Put(catId, catDto);

            //Assert  
            Assert.IsType<OkObjectResult>(updatedData);
        }

        //-------------------------------------------------------------
        //Delete - Deleta categoria por id - Retorna CategoriaDTO
        [Fact]
        public async void Delete_Categoria_Return_OkResult()
        {
            //Arrange  
            var controller = new CategoriasController(_repository, _mapper);
            var catId = 5;

            //Act  
            var data = await controller.Delete(catId);

            //Assert  
            Assert.IsType<OkObjectResult>(data);
        }

    }
}