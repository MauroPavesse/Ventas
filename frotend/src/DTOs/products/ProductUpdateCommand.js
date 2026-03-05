export class ProductUpdateCommand {
    constructor({
        Id = 0,
        Code = 0 | null,
        Name = "",
        Description = "",
        ImagePath = "",
        Price = 0,
        CodeBar = "",
        CategoryId = 0,
        TaxRateId = 0
    } = {}) {
        this.Id = Id;
        this.Code = Code;
        this.Name = Name;
        this.Description = Description;
        this.ImagePath = ImagePath;
        this.Price = Price;
        this.CodeBar = CodeBar;
        this.CategoryId = CategoryId;
        this.TaxRateId = TaxRateId;
    }
}