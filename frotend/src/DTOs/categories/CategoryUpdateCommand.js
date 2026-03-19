export class CategoryUpdateCommand {
    constructor({
        id = 0,
        name = ""
    } = {}) {
        this.id = id;
        this.name = name;
    }
}