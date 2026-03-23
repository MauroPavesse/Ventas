using MediatR;
using System.Data;
using System.Linq.Expressions;
using Ventas.Application.Entities.Externas.Jwt;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Users.Login
{
    public record LoginServiceCommand(string Username, string Password) : IRequest<LoginServiceOutput>;
    
    public class LoginServiceHandler : IRequestHandler<LoginServiceCommand, LoginServiceOutput>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public LoginServiceHandler(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<LoginServiceOutput> Handle(LoginServiceCommand request, CancellationToken cancellationToken)
        {
            // 1. Buscar solo por nombre de usuario (sin la password en el predicado)
            Expression<Func<User, bool>> predicate = t =>
                t.Deleted == 0 && t.Username.Equals(request.Username);

            var user = (await _userRepository.SearchAsync(predicate)).FirstOrDefault();

            // 2. Verificar usuario y comparar hash (BCrypt)
            // Importante: No digas "Usuario no existe", di "Datos incorrectos" por seguridad.
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Credenciales inválidas.");
            }

            var token = _jwtService.GenerateToken(user);

            return new LoginServiceOutput()
            {
                UserId = user.Id,
                Token = token,
                UserName = user.Username
            };
        }
    }
}
