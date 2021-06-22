import { NgModule, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Rent } from '../../../_models/Rent';
import { UserService } from '../../../_services/user.service';
import { BookService } from '../../../_services/book.service';
import { AppComponent } from '../../../app.component';

@Component({
  selector: 'app-rent-borrow',
  templateUrl: './current-rent.component.html',
  providers: [AppComponent]
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
  userType: String;
  message: String;
  submitted: bool;

  constructor(
    private userService: UserService,
    private bookService: BookService,
    private app: AppComponent,) {
  }

  ngOnInit() {
    if (this.app.IsExpired("l,r"))
      return;
    this.userType = this.app.GetUserType();
    this.GetRent();
    this.submitted = true;
  }

  GetRent() {
    this.submitted = false;
    if (this.app.IsExpired("l,r"))
      return;
    if (this.app.GetUserType() == "r") {
      localStorage.setItem("userId", this.app.GetUserId())
    }
    this.userService.GetRent(localStorage.getItem("userId")).subscribe((rents: any[]) => this.rent = rents,
      response => { this.message = (<any>response).error });
    this.submitted = true;
  }

  ReturnBook2(rentId) {
    this.submitted = false;
    if (this.app.IsExpired("l"))
      return;
    this.bookService.ReturnBook(rentId).subscribe(data => {
      this.rent = this.rent.filter(rent => rent.rentId != rentId);
      this.message = "Książka została poprawnie zwrócona";
    },
      response => { this.message = (<any>response).error });
    this.submitted = true;
  }

  ReturnBook(rentId) {
    this.submitted = false;
    if (this.app.IsExpired("l"))
      return;
    this.bookService.ReturnBook(rentId).subscribe(data => {
      this.rent = this.rent.filter(rent => rent.rentId != rentId);
      this.message = (<any>data).message; //"Książka została poprawnie zwrócona";
    },
      response => { this.message = (<any>response).error });
    this.submitted = true;
  }
}
