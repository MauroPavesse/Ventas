using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.Users.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Users.Search
{
    public record UserSearchCommand(SearchCommand Search) : IRequest<IEnumerable<UserOutput>>;

    public class UserSearchHandler : IRequestHandler<UserSearchCommand, IEnumerable<UserOutput>>
    {
        private readonly IUserRepository _userRepository;

        public UserSearchHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserOutput>> Handle(UserSearchCommand request, CancellationToken cancellationToken)
        {
            var search = request.Search;

            Expression<Func<User, bool>> predicate = t => t.Deleted == 0;

            if (search.Id > 0)
            {
                predicate = t => t.Id == search.Id;
            }
            else
            {
                var roleIdFilter = search.Filters.FirstOrDefault(t => t.Field == "RoleId");
                if (roleIdFilter != null)
                {
                    predicate = predicate.And(t => t.RoleId == Convert.ToInt32(roleIdFilter.Value));
                }

                var pointOfSaleIdFilter = search.Filters.FirstOrDefault(t => t.Field == "PointOfSaleId");
                if (pointOfSaleIdFilter != null)
                {
                    predicate = predicate.And(t => t.PointOfSaleId == Convert.ToInt32(pointOfSaleIdFilter.Value));
                }
            }

            var users = await _userRepository.SearchAsync(predicate, search.Includes, search.DisableTracking);

            return users.Adapt<IEnumerable<UserOutput>>();
        }
    }
}
