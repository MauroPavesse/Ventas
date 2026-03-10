export class ConfigurationUpdateCommand {
    constructor({
        id = 0,
        stringValue = "",
        numericValue = 0,
        boolValue = false
    } = {}) {
        this.id = id;
        this.stringValue = stringValue;
        this.numericValue = numericValue;
        this.boolValue = boolValue;
    }
}