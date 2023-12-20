using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }


        [HttpGet("menorPreco")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosPreco()
        {
            try
            {
                var produtos = _uow.ProdutoRepository.GetProdutosPorPreco().ToList();
                var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

                return produtosDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutos()
        {
            try
            {
                var produtos = _uow.ProdutoRepository.Get().ToList();

                if (produtos is null)
                    return NotFound("Produtos não encontrados!");

                var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

                return produtosDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> GetProduto(int id)
        {
            try
            {
                var produto = _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);

                if (produto is null)
                    return NotFound("Produto não encontrado!");

                var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

                return produtoDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpPost]
        public ActionResult Post(ProdutoDTO produtoDTO)
        {
            try
            {
                var produto = _mapper.Map<Produto>(produtoDTO);

                if (produto is null)
                    return BadRequest("Dados inválidos!");

                _uow.ProdutoRepository.Add(produto);
                _uow.Commit();

                var produtoDTOReturn = _mapper.Map<ProdutoDTO>(produto);

                return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produtoDTOReturn);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult Put(int id, ProdutoDTO produtoDTO)
        {
            try
            {
                if (id != produtoDTO.ProdutoId)
                    return BadRequest("Dados inválidos!");

                var produto = _mapper.Map<Produto>(produtoDTO);

                _uow.ProdutoRepository.Update(produto);
                _uow.Commit();

                var produtoDTOReturn = _mapper.Map<ProdutoDTO>(produto);

                return Ok(produtoDTOReturn);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            try
            {
                var produto = _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);

                if (produto is null)
                    return NotFound("Produto não encontrado!");

                _uow.ProdutoRepository.Delete(produto);
                _uow.Commit();

                var produtoDTOReturn = _mapper.Map<ProdutoDTO>(produto);

                return Ok(produtoDTOReturn);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }
    }
}