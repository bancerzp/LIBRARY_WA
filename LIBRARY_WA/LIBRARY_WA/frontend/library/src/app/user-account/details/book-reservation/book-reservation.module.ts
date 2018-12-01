import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reservation } from '../../../_models/reservation';
import { UserService } from '../../../_services/user.service';
import { AppComponent } from '../../../app.component';
import { BookService } from '../../../_services/book.service';

@Component({
  selector: 'app-book-reservation',
  templateUrl: './book-reservation.component.html',
  providers: [AppComponent]
})
@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ]
})
export class BookReservationModule {
  reservation: Reservation[];
  column = ["Id. rezerwacji", "ISBN", "Tytuł", "Numer egzemplarza", "Data rezerwacji", "Rezerwacja do:","Kolejka", "Czy gotowe do wypożyczenia?"];
  user_type: string;
  submitted: boolean;
  message: String;

  constructor(
    private userService: UserService,
    private app: AppComponent,
    private bookService: BookService) {
  }

  ngOnInit() {
    
    this.user_type = localStorage.getItem("user_type");
    this.GetReservation();
    this.submitted = true;
  }

  RentBook(reservation_id) {
    this.submitted = false;
    if (this.app.IsExpired())
      return;
    //błędy wyłapać
    this.bookService.RentBook(reservation_id).subscribe(data => {
        this.reservation = this.reservation.filter(reservation => reservation.reservation_id != reservation_id);
          this.message = "Książka została poprawnie wypożyczona";

        },
      response => { this.message = (<any>response).error });
    this.submitted = true;}
  
       
 //    this.bookService.RentBook(reservation_d).map((response: Response) => {
//  alert("Książka została poprawnie wypożyczona")
//}, (Error: Response) => { alert(Error.json); alert("Błąd wypożyczania książki" + alert(JSON.stringify(Error.json))); alert("Błąd wypożyczania książki" + Error.json) });



 //   this.bookService.RentBook(reservation_d).subscribe(data => {
 //     alert("Książka została poprawnie wypożyczona")
 //   },
 //     (Error: Response) => { alert("Błąd wypożyczania książki" + Error); alert("Błąd wypożyczania książki" + alert(JSON.stringify(Error.json())); alert("Błąd wypożyczania książki" + Error.json) });
 


  GetReservation() {
    if (this.app.IsExpired())
      return;
    this.userService.GetReservation(localStorage.getItem("user_id")).subscribe((reservations: any[]) => this.reservation = reservations,
      response => { this.message = (<any>response).error.alert });
  }

  CancelReservation(reservationId) {
    if (this.app.IsExpired())
      return;
    this.submitted = false;
    this.userService.CancelReservation(reservationId).subscribe(data => {
      this.reservation = this.reservation.filter(reservation => reservation.reservation_id != reservationId);
      this.message="Rezerwacja została anulowana";
    },
      response => { this.message = (<any>response).error.alert });
    this.submitted = true;
  }
}
