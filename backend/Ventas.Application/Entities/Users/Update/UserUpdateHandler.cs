using Mapster;
using MediatR;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.Users.DTOs;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Users.Update
{
    public record UserUpdateCommand(int Id, string Username, string? Password, int? RoleId, int? PointOfSaleId) : IRequest<UserOutput>;

    public class UserUpdateHandler : IRequestHandler<UserUpdateCommand, UserOutput>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public UserUpdateHandler(IUserRepository userRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _userRepository = userRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<UserOutput> Handle(UserUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.Id);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"Usuario con Id {request.Id} no encontrado.");
            }
            existingUser.Username = request.Username;

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            }

            existingUser.RoleId = request.RoleId;
            existingUser.PointOfSaleId = request.PointOfSaleId;
            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedUser.Adapt<UserOutput>();
        }
    }
}
