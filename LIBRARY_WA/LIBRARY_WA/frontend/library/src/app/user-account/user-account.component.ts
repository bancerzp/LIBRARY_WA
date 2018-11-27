import { Component, OnInit, NgModule } from '@angular/core';
import { AppModule, appRoutes } from '../app.module'
import { RouterModule } from '@angular/router';
import { Renth } from '../_models/Renth';
import { Rent } from '../_models/Rent';
import { Reservation } from '../_models/reservation';
import { User } from '../_models/User';

@Component({
  selector: 'app-user-account',
  templateUrl: './user-account.component.html',
  styleUrls: ['./user-account.component.css']
})

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)]
})
export class UserAccountComponent implements OnInit {
  rentData: Rent[];
  renthData: Renth[];
  reservationData: Reservation[];
  userData: User[];
  constructor() { }

  ngOnInit() {
    this.GetUser();
  //  this.GetRent();
    this.GetReservation();
  //  this.GetRenth();
  }

  GetUser() {
  }
  
  GetReservation() {

  }

  

}
