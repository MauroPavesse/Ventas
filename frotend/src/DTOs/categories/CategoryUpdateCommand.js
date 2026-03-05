export class CategoryUpdateCommand {
    constructor({
        Id = 0,
        Name = ""
    } = {}) {
        this.Id = Id;
        this.Name = Name;
    }
}