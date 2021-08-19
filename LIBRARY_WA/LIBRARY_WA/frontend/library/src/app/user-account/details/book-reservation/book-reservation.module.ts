import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reservation } from '../../../_models/reservation';
import { UserService } from '../../../_services/user.service';
import { AppComponent } from '../../../app.component';
import { BookService } from '../../../_services/book.service';
import { RentService } from '../../../_services/rent.service';
import { ReservationService } from '../../../_services/reservation.service';

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
  column = ["Id. rez.", "ISBN", "Tytuł", "Numer egzemplarza", "Data rezerwacji", "Rezerwacja do","Kolejka", "Czy gotowe do odbioru?"];
  userType: string;
  submitted: Boolean;
  message: String;

  constructor(
    private userService: UserService,
    private app: AppComponent,
    private bookService: BookService,
    private rentService: RentService,
    private reservationService: ReservationService) {
  }

  ngOnInit() {
    if (this.app.IsExpired("l,r"))
      return;
    this.userType = localStorage.getItem("userType");
    this.GetReservation();
    this.submitted = true;
  }

  RentBook(reservationId) {
    this.submitted = false;
    if (this.app.IsExpired("l"))
      return;
    //błędy wyłapać
    this.rentService.RentBook(reservationId).subscribe(data => {
        this.reservation = this.reservation.filter(reservation => reservation.reservationId != reservationId);
          this.message = "Książka została poprawnie wypożyczona";
        },
      response => { this.message = (<any>response).error });
    this.submitted = true;}
  
  GetReservation() {
    if (this.app.IsExpired("l,r"))
      return;
   
    if (this.app.GetUserType() == "r") {
      localStorage.setItem("userId", this.app.GetUserId()) 
    }
    this.reservationService.GetReservation(localStorage.getItem("userId")).subscribe((reservations: any[]) => this.reservation = reservations,
      response => { this.message = (<any>response).error.alert });
  }

  CancelReservation(reservationId) {
    if (this.app.IsExpired("l,r"))
      return;
    this.submitted = false;
    this.reservationService.CancelReservation(reservationId).subscribe(data => {
      this.reservation = this.reservation.filter(reservation => reservation.reservationId != reservationId);
      this.message="Rezerwacja została anulowana";
    },
      response => { this.message = (<any>response).error.alert });
    this.submitted = true;
  }
}
