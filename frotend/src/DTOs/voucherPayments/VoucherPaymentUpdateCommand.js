export class VoucherPaymentUpdateCommand {
    constructor({
        Id = 0,
        Amount = 0,
        VoucherId = 0,
        PaymentMethodId = 0
    } = {}) {
        this.Id = Id;
        this.Amount = Amount;
        this.VoucherId = VoucherId;
        this.PaymentMethodId = PaymentMethodId;
    }
}