export class PointOfSaleCreateCommand {
    constructor({
        Name = "",
        Number = "",
        Address = "",
        City = "",
        Provincie = "",
        PostalCode = ""
    } = {}) {
        this.Name = Name;
        this.Number = Number;
        this.Address = Address;
        this.City = City;
        this.Provincie = Provincie;
        this.PostalCode = PostalCode;
    }
}