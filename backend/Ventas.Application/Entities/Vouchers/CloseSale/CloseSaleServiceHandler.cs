using Mapster;
using MediatR;
using System.Data;
using System.Linq.Expressions;
using Ventas.Application.Entities.PointOfSaleVoucherTypes;
using Ventas.Application.Entities.Products;
using Ventas.Application.Entities.TaxRates;
using Ventas.Application.Entities.UnitOfWork;
using Ventas.Application.Entities.Users;
using Ventas.Application.Entities.VoucherDetails;
using Ventas.Application.Entities.Vouchers.DTOs;
using Ventas.Domain.Entities;
using Ventas.Domain.Enums;

namespace Ventas.Application.Entities.Vouchers.CloseSale
{
    public record CloseSaleServiceItemCommand(int Id, decimal Quantity, decimal Price, int TaxRateId);

    public record CloseSaleServicePaymentCommand(int Id, decimal Discount, decimal Amount);

    public record CloseSaleServiceCommand(int Id, int Number, List<CloseSaleServiceItemCommand> Items, CloseSaleServicePaymentCommand? Payment, int UserId, int? CustomerId, int VoucherTypeId, int StateEntityId) : IRequest<VoucherOutput>;

    public class CloseSaleServiceHandler : IRequestHandler<CloseSaleServiceCommand, VoucherOutput>
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly ITaxRateRepository _taxRateRepository;
        private readonly IPointOfSaleVoucherTypeRepository _pointOfSaleVoucherTypeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IVoucherDetailRepository _voucherDetailRepository;
        private readonly IProductRepository _productRepository;

        public CloseSaleServiceHandler(IUnitOfWorkRepository unitOfWorkRepository, IVoucherRepository voucherRepository, ITaxRateRepository taxRateRepository, IPointOfSaleVoucherTypeRepository pointOfSaleVoucherTypeRepository, IUserRepository userRepository, IVoucherDetailRepository voucherDetailRepository, IProductRepository productRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _voucherRepository = voucherRepository;
            _taxRateRepository = taxRateRepository;
            _pointOfSaleVoucherTypeRepository = pointOfSaleVoucherTypeRepository;
            _userRepository = userRepository;
            _voucherDetailRepository = voucherDetailRepository;
            _productRepository = productRepository;
        }

        public async Task<VoucherOutput> Handle(CloseSaleServiceCommand request, CancellationToken cancellationToken)
        {
            var taxRates = await _taxRateRepository.GetAllAsync();

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null) throw new DataException("Usuario no encontrado.");

            var productsIds = request.Items.Select(t => t.Id).Distinct().ToList();
            var products = await _productRepository.SearchAsync(
                predicate: t => productsIds.Contains(t.Id));

            Expression<Func<PointOfSaleVoucherType, bool>> predicatePointOfSaleVoucherType = t =>
                    t.Deleted == 0 &&
                    t.VoucherTypeId == request.VoucherTypeId &&
                    t.PointOfSaleId == user.PointOfSaleId;

            var posVoucherType = (await _pointOfSaleVoucherTypeRepository.SearchAsync(predicatePointOfSaleVoucherType)).FirstOrDefault();
            if (posVoucherType == null) throw new Exception("Configuración de punto de venta no encontrada.");

            Voucher? voucher;

            if (request.Id > 0)
            {
                var existingVouchers = await _voucherRepository.SearchAsync(t => t.Id == request.Id);
                voucher = existingVouchers.FirstOrDefault();

                if (voucher == null) throw new DataException("El comprobante a actualizar no existe.");

                var itemsToRemove = voucher.VoucherDetails
                    .Where(old => !request.Items.Any(n => n.Id == old.ProductId))
                    .ToList();

                foreach (var item in itemsToRemove)
                {
                    await _voucherDetailRepository.DeleteAsync(item);
                    voucher.VoucherDetails.Remove(item);
                }

                foreach (var itemRequest in request.Items)
                {
                    var product = products.FirstOrDefault(t => t.Id == itemRequest.Id);
                    if (product == null) continue;

                    var taxRate = taxRates.First(t => t.Id == product.TaxRateId);

                    decimal amountFinal = itemRequest.Quantity * itemRequest.Price;
                    decimal netAmount = amountFinal / (1 + (taxRate.Percentage / 100));
                    decimal discountAmount = request.Payment != null ? (request.Payment.Discount * amountFinal / 100) : 0;

                    var existingItem = voucher.VoucherDetails
                        .FirstOrDefault(d => d.ProductId == itemRequest.Id);

                    if (existingItem != null)
                    {
                        // ACTUALIZAR ítem existente
                        existingItem.Quantity = itemRequest.Quantity;
                        existingItem.PriceUnit = itemRequest.Price;
                        existingItem.AmountNet = netAmount;
                        existingItem.AmountFinal = amountFinal;
                        existingItem.Discount = discountAmount;
                        await _voucherDetailRepository.UpdateAsync(existingItem);
                    }
                    else
                    {
                        // AGREGAR nuevo ítem
                        voucher.VoucherDetails.Add(new VoucherDetail
                        {
                            VoucherId = voucher.Id,
                            ProductId = itemRequest.Id,
                            Quantity = itemRequest.Quantity,
                            PriceUnit = itemRequest.Price,
                            AmountNet = netAmount,
                            AmountFinal = amountFinal,
                            Discount = discountAmount
                        });
                    }
                }

                voucher.AmountNet = voucher.VoucherDetails.Sum(d => d.AmountNet);
                voucher.AmountVAT = voucher.VoucherDetails.Sum(d => d.AmountFinal) - voucher.AmountNet;
                voucher.StateEntityId = request.StateEntityId;
                voucher.CustomerId = request.CustomerId;

                int number = request.Number;
                if (request.StateEntityId == (int)StateEntityEnum.VoucherStateEnum.FINALIZADO && voucher.Number == 0)
                {
                    posVoucherType.Numeration++;
                    number = posVoucherType.Numeration;
                    await _pointOfSaleVoucherTypeRepository.UpdateAsync(posVoucherType);
                }
                voucher.Number = number;

                await _voucherRepository.UpdateAsync(voucher);
            }
            else
            {
                List<VoucherDetail> voucherDetails = new List<VoucherDetail>();
                foreach (var item in request.Items)
                {
                    var product = products.FirstOrDefault(t => t.Id == item.Id);
                    if (product == null) continue;

                    var taxRate = taxRates.First(t => t.Id == product.TaxRateId);

                    decimal amountFinal = item.Quantity * item.Price;
                    decimal discountAmount = request.Payment != null ? (request.Payment.Discount * amountFinal / 100) : 0;
                    decimal netAmount = amountFinal / (1 + (taxRate.Percentage / 100));

                    voucherDetails.Add(new VoucherDetail()
                    {
                        ProductId = item.Id,
                        Quantity = item.Quantity,
                        PriceUnit = item.Price,
                        AmountNet = netAmount,
                        Discount = discountAmount,
                        AmountFinal = amountFinal
                    });
                }

                List<VoucherPayment> voucherPayments = new List<VoucherPayment>();
                if (request.Payment != null)
                {
                    voucherPayments.Add(new VoucherPayment()
                    {
                        Amount = voucherDetails.Sum(t => t.AmountFinal),//request.Payment.Amount,
                        PaymentMethodId = request.Payment.Id
                    });
                }

                int number = 0;
                if(request.StateEntityId == (int)StateEntityEnum.VoucherStateEnum.FINALIZADO)
                {
                    posVoucherType.Numeration++;
                    number = posVoucherType.Numeration;
                    await _pointOfSaleVoucherTypeRepository.UpdateAsync(posVoucherType);
                }

                voucher = new Voucher()
                {
                    Number = number,
                    AmountNet = voucherDetails.Sum(t => t.AmountNet),
                    AmountVAT = voucherDetails.Sum(t => t.AmountFinal) - voucherDetails.Sum(t => t.AmountNet),
                    CAE = string.Empty,
                    CAEExpiration = null,
                    UserId = request.UserId,
                    CustomerId = request.CustomerId,
                    VoucherTypeId = request.VoucherTypeId,
                    DailyBoxId = null,
                    DateCreation = DateTime.Now,
                    StateEntityId = request.StateEntityId,
                    VoucherDetails = voucherDetails,
                    VoucherPayments = voucherPayments
                };

                voucher = await _voucherRepository.CreateAsync(voucher);
            }

            await _unitOfWorkRepository.SaveChangesAsync();
            return voucher.Adapt<VoucherOutput>();
        }
    }
}
