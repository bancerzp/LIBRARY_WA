import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Reservation } from '../../../_models/reservation';

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
  column = ["Id. rezerwacji", "ISBN", "Tytuł", "Numer egzemplarza", "Data rezerwacji", "Rezerwacja do:", "Czy gotowe do wypożyczenia?","Anuluj rezerwację"];


  CancelReservation(reservationId, userId) {

  }
 // @Input
}
