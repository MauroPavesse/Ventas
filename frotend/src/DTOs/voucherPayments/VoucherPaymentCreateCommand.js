export class VoucherPaymentCreateCommand {
    constructor({
        Amount = 0,
        VoucherId = 0,
        PaymentMethodId = 0
    } = {}) {
        this.Amount = Amount;
        this.VoucherId = VoucherId;
        this.PaymentMethodId = PaymentMethodId;
    }
}