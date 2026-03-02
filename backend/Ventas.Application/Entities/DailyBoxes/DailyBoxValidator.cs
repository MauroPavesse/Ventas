using FluentValidation;
using Ventas.Domain.Entities;

namespace Ventas.Application.Entities.DailyBoxes
{
    public class DailyBoxValidator : AbstractValidator<DailyBox>
    {
        public DailyBoxValidator()
        {
        }
    }
}
