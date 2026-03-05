export class PointOfSaleVoucherTypeCreateCommand {
    constructor({
        PointOfSaleId = 0,
        VoucherTypeId = 0,
        Numeration = 0
    } = {}) {
        this.PointOfSaleId = PointOfSaleId;
        this.VoucherTypeId = VoucherTypeId;
        this.Numeration = Numeration;
    }
}