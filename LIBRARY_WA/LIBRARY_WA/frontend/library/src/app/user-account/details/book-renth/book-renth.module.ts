import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Renth } from '../../../_models/Renth';

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
  
}
