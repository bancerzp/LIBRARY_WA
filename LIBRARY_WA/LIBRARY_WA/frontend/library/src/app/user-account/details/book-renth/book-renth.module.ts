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
  name:String

  constructor(
    private userService: UserService,
    private app: AppComponent,) {
  }

  ngOnInit() {
    this.GetRenth();
  }

  GetRenth() {
    if (this.app.IsExpired())
      return;
    this.name = localStorage.getItem("user_id");
    this.userService.GetRenth(this.name).subscribe((renths: any[]) => this.renth = renths);
  }
}
