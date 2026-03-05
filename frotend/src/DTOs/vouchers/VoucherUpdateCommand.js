export class VoucherUpdateCommand {
    constructor({
        Id = 0,
        Number = 0,
        AmountNet = 0,
        AmountVAT = 0,
        CAE = "",
        CAEExpiration = Date.now(),
        UserId = 0,
        CustomerId = 0 | null,
        VoucherTypeId = 0,
        DailyBoxId = 0 | null,
        DateCreation = Date.now(),
        StateEntityId = 0
    } = {}) {
        this.Id = Id;
        this.Number = Number,
        this.AmountNet = AmountNet;
        this.AmountVAT = AmountVAT;
        this.CAE = CAE;
        this.CAEExpiration = CAEExpiration;
        this.UserId = UserId;
        this.CustomerId = CustomerId;
        this.VoucherTypeId = VoucherTypeId;
        this.DailyBoxId = DailyBoxId;
        this.DateCreation = DateCreation;
        this.StateEntityId = StateEntityId;
    }
}