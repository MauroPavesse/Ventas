using Mapster;
using MediatR;
using Ventas.Application.Entities.AfipTokens.DTOs;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.AfipTokens.Create
{
    public record AfipTokenCreateCommand(string Token, string Sign, DateTimeOffset ExpirationDate) : IRequest<AfipTokenOutput>;

    public class AfipTokenCreateHandler : IRequestHandler<AfipTokenCreateCommand, AfipTokenOutput>
    {
        private readonly IAfipTokenRepository _afipTokenRepository; 
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public AfipTokenCreateHandler(IAfipTokenRepository afipTokenRepository, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _afipTokenRepository = afipTokenRepository;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public async Task<AfipTokenOutput> Handle(AfipTokenCreateCommand request, CancellationToken cancellationToken)
        {
            var afipToken = await _afipTokenRepository.CreateAsync(request.Adapt<AfipToken>());
            await _unitOfWorkRepository.SaveChangesAsync();
            return afipToken.Adapt<AfipTokenOutput>();
        }
    }
}
