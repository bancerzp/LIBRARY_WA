import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Renth } from '../../../_models/Renth';
import { UserService } from '../../../_services/user.service';
import { AppComponent } from '../../../app.component';

@Component({
  selector: 'app-book-renth',
  templateUrl: './book-renth.component.html',
  providers: [AppComponent],
})

@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ]
})
export class BookRenthModule {
  renth: Renth[];
  column = ["Numer wypożyczenia", "ISBN", "Tytuł", "Numer egzemplarza", "Data wypożyczenia", "Data zwrotu"];
  name: String;
  message: String;

  constructor(
    private userService: UserService,
    private app: AppComponent,) {
  }

  ngOnInit() {
    if (this.app.IsExpired("l,r"))
      return;
    this.GetRenth();
  }

  GetRenth() {
    if (this.app.IsExpired("l,r"))
      return;
    if (this.app.GetUserType() == "l") {
      this.name = localStorage.getItem("user_id");
    }
    else {
     this.name = this.app.GetUserId();
    }
    this.userService.GetRenth(this.name).subscribe((renths: any[]) => this.renth = renths,
      response => { this.message = (<any>response).error.alert });
  }
}
