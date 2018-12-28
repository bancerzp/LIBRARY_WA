export class Status {

  user_id: Number;
  status: boolean;

  constructor(
    user_id: Number,
    status: boolean) {
    this.user_id = user_id;
    this.status=status;
  }
}

