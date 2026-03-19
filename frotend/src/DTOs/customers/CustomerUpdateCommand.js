export class CustomerUpdateCommand {
    constructor({
        id = 0,
        document = 0,
        cuit = "",
        firstName = "",
        lastName = "",
        taxConditionId = 0
    } = {}) {
        this.id = id;
        this.document = document;
        this.cuit = cuit;
        this.firstName = firstName;
        this.lastName = lastName;
        this.taxConditionId = taxConditionId;
    }
}