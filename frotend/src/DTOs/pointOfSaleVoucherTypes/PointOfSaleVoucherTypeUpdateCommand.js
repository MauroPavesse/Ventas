export class PointOfSaleVoucherTypeUpdateCommand {
    constructor({
        Id = 0,
        PointOfSaleId = 0,
        VoucherTypeId = 0,
        Numeration = 0
    } = {}) {
        this.Id = Id;
        this.PointOfSaleId = PointOfSaleId;
        this.VoucherTypeId = VoucherTypeId;
        this.Numeration = Numeration;
    }
}