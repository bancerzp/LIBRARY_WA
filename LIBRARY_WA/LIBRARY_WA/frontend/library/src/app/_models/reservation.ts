export class Reservation {

  reservation_id: Int32Array;
  book_id: Int32Array;
  user_id: Int32Array;
  isbn: String;
  title: String;
  volume_id: Int32Array;
  start_date: Date;
  expire_date: Date;
  queue: Int32Array;
  is_active: Boolean;
}
