using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.Roles.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Roles.Search
{
    public record RoleSearchCommand(SearchCommand Search) : IRequest<IEnumerable<RoleOutput>>;


    public class RoleSearchHandler : IRequestHandler<RoleSearchCommand, IEnumerable<RoleOutput>>
    {
        private readonly IRoleRepository _roleRepository;

        public RoleSearchHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleOutput>> Handle(RoleSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<Role, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }

            var roles = await _roleRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return roles.Adapt<IEnumerable<RoleOutput>>();
        }

    }
}
