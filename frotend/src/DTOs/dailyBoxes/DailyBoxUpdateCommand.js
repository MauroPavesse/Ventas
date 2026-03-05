export class DailyBoxUpdateCommand {
    constructor({
        Id = 0,
        Number = 0,
        Amount = 0
    } = {}) {
        this.Id = Id;
        this.Number = Number;
        this.Amount = Amount;
    }
}