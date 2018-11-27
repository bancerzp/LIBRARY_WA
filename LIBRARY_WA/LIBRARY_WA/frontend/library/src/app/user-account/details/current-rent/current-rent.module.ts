import { NgModule, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Rent } from '../../../_models/Rent';
import { UserService } from '../../../_services/user.service';

@Component({
  selector: 'app-rent-borrow',
  templateUrl: './current-rent.component.html'
})

@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ]
})
export class CurrentRentModule {
  rent: Rent[];
  column = ["Numer wypożyczenia", "ISBN", "Tytuł", "Numer egzemplarza", "Data wypożyczenia", "Wypożyczenie do"];
  

  constructor(
    private userService: UserService) {
  }

  ngOnInit() {
    this.GetRent();
  }

  GetRent() {
    this.userService.GetRent(localStorage.getItem("user_id")).subscribe((rents: any[]) => this.rent = rents);
  }
}
