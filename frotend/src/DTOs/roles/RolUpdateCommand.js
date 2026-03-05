export class RolUpdateCommand {
    constructor({
        Id = 0,
        Name = ""
    } = {}) {
        this.Id = Id;
        this.Name = Name;
    }
}