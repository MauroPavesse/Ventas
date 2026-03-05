export class UserCreateCommand {
    constructor({
        Username = "",
        Password = "",
        RoleId = 0 | null,
        PointOfSaleId = 0 | null
    } = {}) {
        this.Username = Username;
        this.Password = Password;
        this.RoleId = RoleId;
        this.PointOfSaleId = PointOfSaleId
    }
}