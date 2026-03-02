using MediatR;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Roles.Delete
{
    public record RoleDeleteCommand(int Id) : IRequest<bool>;

    public class RoleDeleteHandler : IRequestHandler<RoleDeleteCommand, bool>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public RoleDeleteHandler(IRoleRepository roleRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _roleRepository = roleRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<bool> Handle(RoleDeleteCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleRepository.GetByIdAsync(request.Id);
            if (existingRole == null)
            {
                throw new KeyNotFoundException($"Rol con ID {request.Id} no encontrado.");
            }
            existingRole.Deleted = 1;
            existingRole.Active = 0;
            var result = await _roleRepository.UpdateAsync(existingRole);
            await _unitOfWorkRepository.SaveChangesAsync();
            return true;
        }
    }
}
