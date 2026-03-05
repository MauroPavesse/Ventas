export class UserUpdateCommand {
    constructor({
        Id = 0,
        Username = "",
        Password = "",
        RoleId = 0 | null,
        PointOfSaleId = 0 | null
    } = {}) {
        this.Id = Id;
        this.Username = Username;
        this.Password = Password;
        this.RoleId = RoleId;
        this.PointOfSaleId = PointOfSaleId
    }
}