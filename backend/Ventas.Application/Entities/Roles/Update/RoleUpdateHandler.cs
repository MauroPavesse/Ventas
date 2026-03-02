using Mapster;
using MediatR;
using Ventas.Application.Entities.Roles.DTOs;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Roles.Update
{
    public record RoleUpdateCommand(int Id, string Name) : IRequest<RoleOutput>;

    public class RoleUpdateHandler : IRequestHandler<RoleUpdateCommand, RoleOutput>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public RoleUpdateHandler(IRoleRepository roleRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _roleRepository = roleRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<RoleOutput> Handle(RoleUpdateCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleRepository.GetByIdAsync(request.Id);
            if (existingRole == null)
            {
                throw new KeyNotFoundException($"Rol con Id {request.Id} no encontrado.");
            }
            existingRole.Name = request.Name;
            var updatedRole = await _roleRepository.UpdateAsync(existingRole);
            await _unitOfWorkRepository.SaveChangesAsync();
            return updatedRole.Adapt<RoleOutput>();
        }
    }
}
