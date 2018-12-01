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

  constructor(
    private userService: UserService,
    private bookService: BookService,
    private app: AppComponent,) {
  }

  ngOnInit() {
    this.user_type = localStorage.getItem("user_type");
    this.GetRent();
  }

  GetRent() {
    if (this.app.IsExpired())
      return;
    this.userService.GetRent(localStorage.getItem("user_id")).subscribe((rents: any[]) => this.rent = rents);
  }
  ReturnBook(rent_id) {
    if (this.app.IsExpired())
      return;
    //błędy wyłapać
    this.bookService.ReturnBook(rent_id).subscribe(data => {
      alert("Książka poprawnie zwrócona")
    },
      Error => { alert("Błąd zwrotu książki" + Error.error); alert("Błąd zwrotu książki" + Error) });
  }
}
