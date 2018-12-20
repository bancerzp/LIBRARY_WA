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
  user_type: String;
  message: String;
  submitted: boolean;

  constructor(
    private userService: UserService,
    private bookService: BookService,
    private app: AppComponent,) {
  }

  ngOnInit() {
    if (this.app.IsExpired("l,r"))
      return;
    this.user_type = this.app.GetUserType();
    this.GetRent();
    this.submitted = true;
  }

  GetRent() {
    this.submitted = false;
    if (this.app.IsExpired("l,r"))
      return;
    if (this.app.GetUserType() == "r") {
      localStorage.setItem("user_id", this.app.GetUserId())
    }
    this.userService.GetRent(localStorage.getItem("user_id")).subscribe((rents: any[]) => this.rent = rents,
      response => { this.message = (<any>response).error });
    this.submitted = true;
  }

  ReturnBook2(rent_id) {
    this.submitted = false;
    if (this.app.IsExpired("l"))
      return;
    //błędy wyłapać
    this.bookService.ReturnBook(rent_id).subscribe(data => {
      this.rent = this.rent.filter(rent => rent.rent_id != rent_id);
      this.message = "Książka została poprawnie zwrócona";
    },
      response => { this.message = (<any>response).error });
    this.submitted = true;
  }

  ReturnBook(rent_id) {
    this.submitted = false;
    if (this.app.IsExpired("l"))
      return;
    //błędy wyłapać
    this.bookService.ReturnBook(rent_id).subscribe(data => {
      this.rent = this.rent.filter(rent => rent.rent_id != rent_id);
      this.message = (<any>data).message; //"Książka została poprawnie zwrócona";
    },
      response => { this.message = (<any>response).error });
    this.submitted = true;
  }
}
