using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microservicio.Login.Api.Aplicacion;

namespace Microservicio.Login.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioControlador : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsuarioControlador(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<ActionResult<List<UsuarioDto>>> GetUsuarios()
        {
            return await _mediator.Send(new ConsultaUsuario.ListaUsuario());
        }

        // Endpoint para buscar por usuario
        [HttpGet("usuario/{usuario}")]
        public async Task<ActionResult<UsuarioDto>> GetPorUsuario(string usuario)
        {
            try
            {
                var result = await _mediator.Send(new ConsultaPorUsuario.Ejecuta
                {
                    Usuario = usuario // Búsqueda exacta (case-sensitive)
                });
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // 404 si no existe
            }
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario([FromBody] NuevoUsuario.Ejecuta data)
        {
            await _mediator.Send(data);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> ActualizarUsuario([FromBody] ActualizarUsuario.EjecutaActualizar data)
        {
            try
            {
                await _mediator.Send(data);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUsu([FromBody] LoginUsuario.EjecutaLogin request)
        {
            try
            {
                var resultado = await _mediator.Send(request); // Devuelve LoginResponseDto

                return Ok(new
                {
                    mensaje = "Login exitoso",
                    usuario = resultado.Usuario,
                    token = resultado.Token,
                    refreshToken = resultado.RefreshToken
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] Logout.CerrarSesion request)
        {
            try
            {
                await _mediator.Send(request);
                return Ok(new { mensaje = "Sesión cerrada correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }



    }
}
