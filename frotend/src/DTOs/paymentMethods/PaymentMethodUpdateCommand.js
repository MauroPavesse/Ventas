export class PaymentMethodUpdateCommand {
    constructor({
        Id = 0,
        Name = "",
        DescountPercentage = 0,
        IncreasePercentage = 0,
        Color = ""
    } = {}) {
        this.Id = Id;
        this.Name = Name;
        this.DescountPercentage = DescountPercentage;
        this.IncreasePercentage = IncreasePercentage;
        this.Color = Color;
    }
}