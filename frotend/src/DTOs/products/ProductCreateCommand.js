export class ProductCreateCommand {
    constructor({
        code = 0 | null,
        name = "",
        description = "",
        imagePath = "",
        price = 0,
        codeBar = "",
        categoryId = 0,
        taxRateId = 0
    } = {}) {
        this.code = code;
        this.name = name;
        this.description = description;
        this.imagePath = imagePath;
        this.price = price;
        this.codeBar = codeBar;
        this.categoryId = categoryId;
        this.taxRateId = taxRateId;
    }
}