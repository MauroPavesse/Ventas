export class UserLoginCommand {
    constructor({
        username = "",
        password = ""
    } = {}) {
        this.username = username;
        this.password = password;
    }
}