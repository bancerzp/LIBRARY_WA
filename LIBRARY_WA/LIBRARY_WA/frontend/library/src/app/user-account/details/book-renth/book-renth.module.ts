import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Renth } from '../../../_models/Renth';
import { UserService } from '../../../_services/user.service';

@Component({
  selector: 'app-book-renth',
  templateUrl: './book-renth.component.html'
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


  constructor(
    private userService: UserService) {
  }

  ngOnInit() {
    this.GetRenth();
  }

  GetRenth() {
    this.userService.GetRenth(localStorage.getItem("user_id")).subscribe((renths: any[]) => this.renth = renths);
  }

}
