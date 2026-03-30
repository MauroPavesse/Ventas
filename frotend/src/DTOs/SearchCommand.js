export class SearchFilterCommand {
  constructor(field = "", value = "", ids = []) {
    this.field = field;
    this.value = value;
    this.ids = ids;
  }
}

export class SearchCommand {
  constructor({
    id = null,
    filters = [],
    includes = [],
    disableTracking = true,
    deleted = 0,
  } = {}) {
    this.id = id;
    this.filters = filters;
    this.includes = includes;
    this.disableTracking = disableTracking;
    this.deleted = deleted;
  }
}
