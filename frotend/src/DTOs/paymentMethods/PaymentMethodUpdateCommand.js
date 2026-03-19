export class PaymentMethodUpdateCommand {
    constructor({
        id = 0,
        name = "",
        descountPercentage = 0,
        increasePercentage = 0,
        color = ""
    } = {}) {
        this.id = id;
        this.name = name;
        this.descountPercentage = descountPercentage;
        this.increasePercentage = increasePercentage;
        this.color = color;
    }
}