using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public CategoriasController(IUnitOfWork uow, ILogger<CategoriasController> logger, IMapper mapper)
        {
            _uow = uow;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpGet("categoriasProdutos")]
        public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriasProdutos()
        {
            try
            {
                _logger.LogInformation("================== GET api/categorias/produtos ======================");

                var categorias = _uow.CategoriaRepository.GetCategoriasProdutos().ToList();

                if (categorias is null)
                    return NotFound("Categorias não encontradas!");

                var categoriasDTO = _mapper.Map<List<CategoriaDTO>>(categorias);

                return categoriasDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>> GetCategorias()
        {
            try
            {
                var categorias = _uow.CategoriaRepository.Get().ToList();

                if (categorias is null)
                    return NotFound("Categorias não encontradas!");

                var categoriasDTO = _mapper.Map<List<CategoriaDTO>>(categorias);

                return categoriasDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> GetCategoria(int id)
        {
            try
            {
                var categoria = _uow.CategoriaRepository.GetById(p => p.CategoriaId == id);

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

        [HttpPost]
        public ActionResult Post(CategoriaDTO categoriaDTO)
        {
            try
            {
                var categoria = _mapper.Map<Categoria>(categoriaDTO);

                if (categoria is null)
                    return BadRequest("Dados inválidos!");

                _uow.CategoriaRepository.Add(categoria);
                _uow.Commit();

                var categoriaDTOReturn = _mapper.Map<CategoriaDTO>(categoria);

                return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoriaDTOReturn);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, CategoriaDTO categoriaDTO)
        {
            try
            {
                if (id != categoriaDTO.CategoriaId)
                    return BadRequest("Dados inválidos!");

                var categoria = _mapper.Map<Categoria>(categoriaDTO);

                _uow.CategoriaRepository.Update(categoria);
                _uow.Commit();

                var categoriaDTOReturn = _mapper.Map<CategoriaDTO>(categoria);

                return Ok(categoriaDTOReturn);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            try
            {
                var categoria = _uow.CategoriaRepository.GetById(p => p.CategoriaId == id);

                if (categoria is null)
                    return NotFound("Categoria não encontrada!");

                _uow.CategoriaRepository.Delete(categoria);
                _uow.Commit();

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