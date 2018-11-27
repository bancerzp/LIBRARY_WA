import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reservation } from '../../../_models/reservation';
import { UserService } from '../../../_services/user.service';

@Component({
  selector: 'app-book-reservation',
  templateUrl: './book-reservation.component.html'
})
@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ]
})
export class BookReservationModule {
  reservation: Reservation[];
  column = ["Id. rezerwacji", "ISBN", "Tytuł", "Numer egzemplarza", "Data rezerwacji", "Rezerwacja do:", "Czy gotowe do wypożyczenia?","Akcje"];
  user_type: string;

  constructor(
    private userService: UserService) {
  }

  ngOnInit() {
    this.user_type = localStorage.getItem("user_type");
    this.GetReservation();
  }

  GetReservation() {
    this.userService.GetReservation(localStorage.getItem("user_id")).subscribe((reservations: any[]) => this.reservation = reservations);
  }
  CancelReservation(reservationId, userId) {

  }
 // @Input
}
