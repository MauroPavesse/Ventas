using Mapster;
using MediatR;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.Users.DTOs;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Users.Create
{
    public record UserCreateCommand(string Username, string Password, int? RoleId, int? PointOfSaleId) : IRequest<UserOutput>;

    public class UserCreateHandler : IRequestHandler<UserCreateCommand, UserOutput>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public UserCreateHandler(IUserRepository userRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _userRepository = userRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<UserOutput> Handle(UserCreateCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.CreateAsync(request.Adapt<User>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return user.Adapt<UserOutput>();
        }
    }
}
