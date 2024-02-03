using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    [ApiController]
    //[EnableCors("PermitisApiRequest")]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        //private readonly ILogger _logger;

        public CategoriasController(IUnitOfWork uow, IMapper mapper /*ILogger<CategoriasController> logger*/)
        {
            _uow = uow;
            _mapper = mapper;
            //_logger = logger;
        }


        [HttpGet("categoriasProdutos")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            try
            {
                //_logger.LogInformation("================== GET api/categorias/produtos ======================");

                var categorias = await _uow.CategoriaRepository.GetCategoriasProdutos();

                if (categorias is null)
                    return NotFound("Categorias não encontradas!");

                var categoriasDTO = _mapper.Map<List<CategoriaDTO>>(categorias);

                return Ok(categoriasDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategorias([FromQuery] CategoriasParameters categoriasParameters)
        {
            try
            {
                var categorias = await _uow.CategoriaRepository.GetCategorias(categoriasParameters);

                if (categorias is null)
                    return NotFound("Categorias não encontradas!");

                var metadata = new
                {
                    categorias.TotalCount,
                    categorias.PageSize,
                    categorias.CurrentPage,
                    categorias.TotalPages,
                    categorias.HasNext,
                    categorias.HasPrevious
                };

                Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

                var categoriasDTO = _mapper.Map<IEnumerable<CategoriaDTO>>(categorias);

                return Ok(categoriasDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        /// <summary>
        /// Obtem uma Categoria pelo seu Id
        /// </summary>
        /// <param name="id">Código da Categoria</param>
        /// <returns>Objetos Categoria</returns>
        [HttpGet("{id:int}", Name = "ObterCategoria")]
        [ProducesResponseType(typeof(ProdutoDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //[EnableCors("PermitisApiRequest")]
        public async Task<ActionResult<CategoriaDTO>> GetCategoria(int id)
        {
            try
            {
                var categoria = await _uow.CategoriaRepository.GetById(p => p.CategoriaId == id);

                if (categoria is null)
                    return NotFound("Categoria não encontrada!");

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return Ok(categoriaDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        /// <summary>
        /// Inclui uma nova Categoria
        /// </summary>
        /// <remarks>
        /// Exemplo de request:
        /// 
        ///     POST api/categorias
        ///     {
        ///         "categoriaId": 1,
        ///         "nome": "categoria1",
        ///         "imagemUrl": "http://teste.net/1.jpg"
        ///     }
        /// </remarks>
        /// <param name="categoriaDTO">Objeto Categoria</param>
        /// <returns>O objeto Categoria incluido</returns>
        /// <remarks>Retorna um objeto Categoria incluído</remarks>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Post([FromBody] CategoriaDTO categoriaDTO)
        {
            try
            {
                var categoria = _mapper.Map<Categoria>(categoriaDTO);

                if (categoria is null)
                    return BadRequest("Dados inválidos!");

                _uow.CategoriaRepository.Add(categoria);
                await _uow.Commit();

                var categoriaDTOReturn = _mapper.Map<CategoriaDTO>(categoria);

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoriaDTOReturn);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpPut("{id:int}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Put))]
        public async Task<ActionResult> Put(int id, CategoriaDTO categoriaDTO)
        {
            try
            {
                if (id != categoriaDTO.CategoriaId)
                    return BadRequest("Dados inválidos!");

                var categoria = _mapper.Map<Categoria>(categoriaDTO);

                _uow.CategoriaRepository.Update(categoria);
                await _uow.Commit();

                var categoriaDTOReturn = _mapper.Map<CategoriaDTO>(categoria);

                return Ok(categoriaDTOReturn);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var categoria = await _uow.CategoriaRepository.GetById(p => p.CategoriaId == id);

                if (categoria is null)
                    return NotFound("Categoria não encontrada!");

                _uow.CategoriaRepository.Delete(categoria);
                await _uow.Commit();

                var categoriaDTOReturn = _mapper.Map<CategoriaDTO>(categoria);

                return Ok(categoriaDTOReturn);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }
    }
}