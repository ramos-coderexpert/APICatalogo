using APICatalogo.DTOs;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPreco()
        {
            try
            {
                var produtos = await _uow.ProdutoRepository.GetProdutosPorPreco();
                var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

                return produtosDTO;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        /// <summary>
        /// Exibe uma relação dos produtos
        /// </summary>
        /// <param name="produtosParameters"></param>
        /// <returns>Retorna uma lista de objetos Produto</returns>
        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> GetProdutos([FromQuery] ProdutosParameters produtosParameters)
        {
            try
            {
                var produtos = await _uow.ProdutoRepository.GetProdutos(produtosParameters);

                if (produtos is null)
                    return NotFound("Produtos não encontrados!");

                var metadata = new
                {
                    produtos.TotalCount,
                    produtos.PageSize,
                    produtos.CurrentPage,
                    produtos.TotalPages,
                    produtos.HasNext,
                    produtos.HasPrevious
                };

                Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

                var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

                return Ok(produtosDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        /// <summary>
        /// Obtem um Produto pelo seu Id
        /// </summary>
        /// <param name="id">Id do Produto</param>
        /// <returns>Um Objeto Produto</returns>
        [HttpGet("{id:int}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> GetProduto(int id)
        {
            try
            {
                var produto = await _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);

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
        public async Task<ActionResult> Post(ProdutoDTO produtoDTO)
        {
            try
            {
                var produto = _mapper.Map<Produto>(produtoDTO);

                if (produto is null)
                    return BadRequest("Dados inválidos!");

                _uow.ProdutoRepository.Add(produto);
                await _uow.Commit();

                var produtoDTOReturn = _mapper.Map<ProdutoDTO>(produto);

                return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produtoDTOReturn);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpPatch("{id:int}/UpdatePartial")]
        public async Task<ActionResult<ProdutoDTOUpdateResponse>> Patch(int id, JsonPatchDocument patchProdutoDTO)
        {
            if (patchProdutoDTO is null || id <= 0)
                return BadRequest();

            var produto = await _uow.ProdutoRepository.GetById(c => c.ProdutoId == id);

            if (produto is null)
                return NotFound();

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDTO.ApplyTo(produtoUpdateRequest);

            if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
                return BadRequest(ModelState);

            _mapper.Map(produtoUpdateRequest, produto);

            _uow.ProdutoRepository.Update(produto);
            await _uow.Commit();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, ProdutoDTO produtoDTO)
        {
            try
            {
                if (id != produtoDTO.ProdutoId)
                    return BadRequest("Dados inválidos!");

                var produto = _mapper.Map<Produto>(produtoDTO);

                _uow.ProdutoRepository.Update(produto);
                await _uow.Commit();

                var produtoDTOReturn = _mapper.Map<ProdutoDTO>(produto);

                return Ok(produtoDTOReturn);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ocorreu um problema com a sua solicitação!");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            try
            {
                var produto = await _uow.ProdutoRepository.GetById(p => p.ProdutoId == id);

                if (produto is null)
                    return NotFound("Produto não encontrado!");

                _uow.ProdutoRepository.Delete(produto);
                await _uow.Commit();

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