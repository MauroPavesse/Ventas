export class UserCreateCommand {
    constructor({
        username = "",
        password = "",
        roleId = 0 | null,
        pointOfSaleId = 0 | null
    } = {}) {
        this.username = username;
        this.password = password;
        this.roleId = roleId;
        this.pointOfSaleId = pointOfSaleId
    }
}