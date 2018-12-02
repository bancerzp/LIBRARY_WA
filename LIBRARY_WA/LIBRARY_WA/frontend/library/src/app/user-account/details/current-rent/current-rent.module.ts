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
    this.user_type = localStorage.getItem("user_type");
    this.GetRent();
    this.submitted = true;
  }

  GetRent() {
   // var decoded = jwt.decode(token);

    let jwtData = localStorage.getItem("token").split('.')[1]
    let decodedJwtJsonData = window.atob(jwtData)
    let decodedJwtData = JSON.parse(decodedJwtJsonData)
    let isAdmin = decodedJwtData.l
    
    this.submitted = false;
    if (this.app.IsExpired())
      return;
    this.userService.GetRent(localStorage.getItem("user_id")).subscribe((rents: any[]) => this.rent = rents,
      response => { this.message = (<any>response).error });
    this.submitted = true;
    this.message = isAdmin;
  }
  ReturnBook2(rent_id) {
    this.submitted = false;
    if (this.app.IsExpired())
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
    if (this.app.IsExpired())
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
