export class PointOfSaleCreateCommand {
    constructor({
        name = "",
        number = "",
        address = "",
        city = "",
        provincie = "",
        postalCode = ""
    } = {}) {
        this.name = name;
        this.number = number;
        this.address = address;
        this.city = city;
        this.provincie = provincie;
        this.postalCode = postalCode;
    }
}