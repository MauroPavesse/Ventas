export class VoucherToInvoiceCommand {
    constructor({
        voucherId = 0
    } = {}) {
        this.voucherId = voucherId;
    }
}