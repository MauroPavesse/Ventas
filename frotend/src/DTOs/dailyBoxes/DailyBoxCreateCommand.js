export class DailyBoxCreateCommand {
    constructor({
        Number = 0,
        Amount = 0
    } = {}) {
        this.Number = Number;
        this.Amount = Amount;
    }
}