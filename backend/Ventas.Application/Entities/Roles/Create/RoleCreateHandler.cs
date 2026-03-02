using Mapster;
using MediatR;
using Ventas.Application.Entities.Roles.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Roles.Create
{
    public record RoleCreateCommand(string Name) : IRequest<RoleOutput>;

    public  class RoleCreateHandler : IRequestHandler<RoleCreateCommand, RoleOutput>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public RoleCreateHandler(IRoleRepository roleRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _roleRepository = roleRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<RoleOutput> Handle(RoleCreateCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.CreateAsync(request.Adapt<Role>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return role.Adapt<RoleOutput>();
        }
    }
}
