export class CustomerUpdateCommand {
    constructor({
        Id = 0,
        Document = 0,
        Cuit = "",
        FirstName = "",
        LastName = "",
        TaxConditionId = 0
    } = {}) {
        this.Id = Id;
        this.Document = Document;
        this.Cuit = Cuit;
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.TaxConditionId = TaxConditionId;
    }
}