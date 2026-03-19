export class UserUpdateCommand {
    constructor({
        id = 0,
        username = "",
        password = "",
        roleId = 0 | null,
        pointOfSaleId = 0 | null
    } = {}) {
        this.id = id;
        this.username = username;
        this.password = password;
        this.roleId = roleId;
        this.pointOfSaleId = pointOfSaleId
    }
}