using Mapster;
using MediatR;
using Ventas.Application.Entities.Configurations.DTOs;
using Ventas.Application.Entities.Externas.Encryption;
using Ventas.Application.Entities.UnitOfWork;

namespace Ventas.Application.Entities.Configurations.Update
{
    public record UpdateConfigurationsBatchCommand(List<ConfigurationUpdateItem> Items) : IRequest<List<ConfigurationOutput>>;

    public record ConfigurationUpdateItem(string Variable, string StringValue = "", decimal NumericValue = 0, bool BoolValue = false);

    public class ConfigurationUpdateHandler : IRequestHandler<UpdateConfigurationsBatchCommand, List<ConfigurationOutput>>
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly IUnitOfWorkRepository unitOfWorkRepository;
        private readonly IEncryptionService encryptionService;

        public ConfigurationUpdateHandler(IConfigurationRepository configurationRepository, IUnitOfWorkRepository unitOfWorkRepository, IEncryptionService encryptionService)
        {
            this.configurationRepository = configurationRepository;
            this.unitOfWorkRepository = unitOfWorkRepository;
            this.encryptionService = encryptionService;
        }

        public async Task<List<ConfigurationOutput>> Handle(UpdateConfigurationsBatchCommand request, CancellationToken cancellationToken)
        {
            var outputs = new List<ConfigurationOutput>();
            var configurations = await configurationRepository.GetAllAsync();
            foreach (var item in request.Items)
            {
                var existingConfiguration = configurations.FirstOrDefault(t => t.Variable == item.Variable);
                if (existingConfiguration == null) continue;

                existingConfiguration.StringValue = item.StringValue;
                existingConfiguration.NumericValue = item.NumericValue;
                existingConfiguration.BoolValue = item.BoolValue;

                if (item.Variable == "arcaClave")
                {
                    existingConfiguration.StringValue = encryptionService.Encrypt(item.StringValue);
                }

                var updatedConfiguration = await configurationRepository.UpdateAsync(existingConfiguration);
                outputs.Add(updatedConfiguration.Adapt<ConfigurationOutput>());
            }
            
            await unitOfWorkRepository.SaveChangesAsync();
            return outputs;
        }
    }
}
