export class PointOfSaleUpdateCommand {
    constructor({
        Id = 0,
        Name = "",
        Number = "",
        Address = "",
        City = "",
        Provincie = "",
        PostalCode = ""
    } = {}) {
        this.Id = Id;
        this.Name = Name;
        this.Number = Number;
        this.Address = Address;
        this.City = City;
        this.Provincie = Provincie;
        this.PostalCode = PostalCode;
    }
}