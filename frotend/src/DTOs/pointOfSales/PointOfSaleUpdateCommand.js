export class PointOfSaleUpdateCommand {
    constructor({
        id = 0,
        name = "",
        number = "",
        address = "",
        city = "",
        provincie = "",
        postalCode = ""
    } = {}) {
        this.id = id;
        this.name = name;
        this.number = number;
        this.address = address;
        this.city = city;
        this.provincie = provincie;
        this.postalCode = postalCode;
    }
}