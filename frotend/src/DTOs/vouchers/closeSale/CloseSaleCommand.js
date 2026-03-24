export class CloseSaleCommand {
    constructor({
        number = 0,
        items = [],
        payment = null,
        userId = 0,
        customerId = null,
        voucherTypeId = 0,
        stateEntityId = 0
    } = {}) {
        this.number = number;
        this.items = items;
        this.payment = payment;
        this.userId = userId;
        this.customerId = customerId;
        this.voucherTypeId = voucherTypeId;
        this.stateEntityId = stateEntityId;
    }
}