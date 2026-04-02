using Mapster;
using MediatR;
using System.Linq.Expressions;
using Ventas.Application.Entities.Configurations.DTOs;
using Ventas.Application.Shared;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.Configurations.Search
{
    public record ConfigurationSearchCommand(List<string> Variables) : IRequest<IEnumerable<ConfigurationOutput>>;

    public class ConfigurationSearchHandler : IRequestHandler<ConfigurationSearchCommand, IEnumerable<ConfigurationOutput>>
    {
        private readonly IConfigurationRepository configurationRepository;

        public ConfigurationSearchHandler(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        public async Task<IEnumerable<ConfigurationOutput>> Handle(ConfigurationSearchCommand request, CancellationToken cancellationToken)
        {
            Expression<Func<Configuration, bool>> predicate = t => t.Deleted == 0;

            if (request.Variables != null && request.Variables.Any())
            {
                // Empezamos con un predicado "falso" para ir sumando ORs
                // O mejor: tomamos la primera variable como base.
                Expression<Func<Configuration, bool>> varPredicate = t => false;

                foreach (var variable in request.Variables)
                {
                    varPredicate = varPredicate.Or(t => t.Variable == variable);
                }

                // 3. Unimos la condición base con el grupo de variables usando AND
                predicate = predicate.And(varPredicate);
            }

            var configurations = await configurationRepository.GetAllAsync(predicate);

            return configurations.Adapt<IEnumerable<ConfigurationOutput>>();
        }
    }
}
