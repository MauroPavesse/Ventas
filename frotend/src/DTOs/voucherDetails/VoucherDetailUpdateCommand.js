export class VoucherDetailUpdateCommand {
    constructor({
        Id = 0,
        VoucherId = 0,
        ProductId = 0,
        Quantity = 0,
        PriceUnit = 0,
        AmountNet = 0,
        Discount = 0,
        AmountFinal = 0
    } = {}) {
        this.Id = Id;
        this.VoucherId = VoucherId;
        this.ProductId = ProductId;
        this.Quantity = Quantity;
        this.PriceUnit = PriceUnit;
        this.AmountNet = AmountNet;
        this.Discount = Discount;
        this.AmountFinal = AmountFinal;
    }
}