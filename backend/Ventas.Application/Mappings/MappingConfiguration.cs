using Mapster;

namespace Ventas.Application.Mappings
{
    public class MappingConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.Default.MaxDepth(3);
        }
    }
}
