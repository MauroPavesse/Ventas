export class CustomerCreateCommand {
    constructor({
        Document = 0,
        Cuit = "",
        FirstName = "",
        LastName = "",
        TaxConditionId = 0
    } = {}) {
        this.Document = Document;
        this.Cuit = Cuit;
        this.FirstName = FirstName;
        this.LastName = LastName;
        this.TaxConditionId = TaxConditionId;
    }
}