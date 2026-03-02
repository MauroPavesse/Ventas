using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Users.Delete
{
    public record UserDeleteCommand(int Id) : IRequest<bool>;

    public class UserDeleteHandler : IRequestHandler<UserDeleteCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public UserDeleteHandler(IUserRepository userRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _userRepository = userRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(UserDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"Usuario con ID {request.Id} no encontrado.");
            }
            existingUser.Deleted = 1;
            existingUser.Active = 0;
            var result = await _userRepository.UpdateAsync(existingUser);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
