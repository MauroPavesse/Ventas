export class CategoryCreateCommand {
    constructor({
        Name = ""
    } = {}) {
        this.Name = Name;
    }
}