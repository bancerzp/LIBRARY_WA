export class Reservation {
    [x: string]: any;
  reservation_id: Number;
  book_id: Number;
  user_id: Number;
  isbn: String;
  title: String;
  volume_id: Number;
  start_date: Date;
  expire_date: Date;
  queue: Number;
  is_active: Boolean;
}
