export class PaymentMethodCreateCommand {
    constructor({
        Name = "",
        DescountPercentage = 0,
        IncreasePercentage = 0,
        Color = ""
    } = {}) {
        this.Name = Name;
        this.DescountPercentage = DescountPercentage;
        this.IncreasePercentage = IncreasePercentage;
        this.Color = Color;
    }
}