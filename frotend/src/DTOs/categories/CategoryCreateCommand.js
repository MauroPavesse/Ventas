export class CategoryCreateCommand {
    constructor({
        name = ""
    } = {}) {
        this.name = name;
    }
}