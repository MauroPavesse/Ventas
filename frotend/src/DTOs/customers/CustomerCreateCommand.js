export class CustomerCreateCommand {
    constructor({
        document = 0,
        cuit = "",
        firstName = "",
        lastName = "",
        taxConditionId = 0
    } = {}) {
        this.document = document;
        this.cuit = cuit;
        this.firstName = firstName;
        this.lastName = lastName;
        this.taxConditionId = taxConditionId;
    }
}