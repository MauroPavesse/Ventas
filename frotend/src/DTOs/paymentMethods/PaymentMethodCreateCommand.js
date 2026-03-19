export class PaymentMethodCreateCommand {
    constructor({
        name = "",
        descountPercentage = 0,
        increasePercentage = 0,
        color = ""
    } = {}) {
        this.name = name;
        this.descountPercentage = descountPercentage;
        this.increasePercentage = increasePercentage;
        this.color = color;
    }
}