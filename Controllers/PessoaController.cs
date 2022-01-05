using AulaRestAPI.Data;
using AulaRestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AulaRestAPI.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController]
    public class PessoaController : ControllerBase
    {
        private readonly IJWTAuthenticationManager jWTAuthenticationManager;

        public PessoaController(IJWTAuthenticationManager jWTAuthenticationManager)
        {
            this.jWTAuthenticationManager = jWTAuthenticationManager;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("pessoas")]
        public async Task<IActionResult> getAllAsync(
            [FromServices] Contexto contexto)
        {
            var pessoas = await contexto
                .Pessoas
                .AsNoTracking()//só pode ser utilizado em consultas - altamente recomendado por questões de desempenho
                .ToListAsync();

            return pessoas == null ? NotFound() : Ok(pessoas);
        }

        [Authorize]
        [HttpGet]
        [Route("pessoas/{id}")]
        public async Task<IActionResult> getByIdAsync(
            [FromServices] Contexto contexto,
            [FromRoute] int id
            )
        {
            var pessoa = await contexto
                .Pessoas.AsNoTracking()
                .FirstOrDefaultAsync(p => p.id == id);

            return pessoa == null ? NotFound() : Ok(pessoa);
        }

        [HttpPost]
        [Route("pessoas")]
        public async Task<IActionResult> PostAsync(
            [FromServices] Contexto contexto,
            [FromBody] Pessoa pessoa
            )
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                await contexto.Pessoas.AddAsync(pessoa);
                await contexto.SaveChangesAsync();
                return Created($"api/pessoas/{pessoa.id}", pessoa);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("pessoas/{id}")]
        public async Task<IActionResult> PutAsync
            (
                [FromServices] Contexto contexto,
                [FromBody] Pessoa pessoa,
                [FromRoute] int id
            )
        {
            if (!ModelState.IsValid)
                return BadRequest("Model inválida");

            var p = await contexto.Pessoas
                .FirstOrDefaultAsync(x => x.id == id);

            if(p == null)
                return NotFound("Pessoa não encontrada!");

            try
            {
                p.nome = pessoa.nome;

                contexto.Pessoas.Update(p);
                await contexto.SaveChangesAsync();
                return Ok(p);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("pessoas/{id}")]
        public async Task<IActionResult> DeleteAsync(
                [FromServices] Contexto contexto,
                [FromRoute] int id)
        {
            var p = await contexto.Pessoas.FirstOrDefaultAsync(x => x.id == id);

            if (p == null)
                return BadRequest("Pessoa não encontrada");

            try
            {
                contexto.Pessoas.Remove(p);
                await contexto.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("autenticar")]
        public IActionResult Authenticate([FromBody] Usuario user)
        {
            var token = jWTAuthenticationManager.Authenticate(user.Username, user.Password);

            if (token == null)
                return Unauthorized();

            return Ok(token);
        }
    }
}
