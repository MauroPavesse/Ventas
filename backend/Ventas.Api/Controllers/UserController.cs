using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ventas.Application.Entities.Users.Create;
using Ventas.Application.Entities.Users.Delete;
using Ventas.Application.Entities.Users.Login;
using Ventas.Application.Entities.Users.Search;
using Ventas.Application.Entities.Users.Update;
using Ventas.Application.Shared;

namespace Ventas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchCommand command) // Cambiado a FromBody
        {
            var result = await _mediator.Send(new UserSearchCommand(command));
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateCommand command)
        {
            var result = await _mediator.Send(command);
            // Devolver 201 es más preciso para creación
            return CreatedAtAction(nameof(Search), new { id = result }, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UserUpdateCommand command)
        {
            await _mediator.Send(command);
            return NoContent(); // O Ok(result) si devuelves el objeto actualizado
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new UserDeleteCommand(id));
            return NoContent(); // 204 No Content es estándar para Delete exitoso
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginServiceCommand command)
        {
            // Tu ExceptionMiddleware se encargará de capturar el error 
            // y devolver 401 si las credenciales fallan.
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
