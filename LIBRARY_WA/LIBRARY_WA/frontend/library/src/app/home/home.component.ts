import { Component, OnInit } from '@angular/core';
import { BookService } from '../_services/book.service';
import { AppComponent } from '../app.component';
import { Suggestion } from '../_models/Suggestion';
import {  SearchBookComponent } from "../search-book/search-book.component"
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  providers: [AppComponent, SearchBookComponent]
})
export class HomeComponent implements OnInit {
  
  suggestion: Suggestion[];
  user_type: String;
  user_id: String;
  cover: number;

  constructor(private bookService: BookService,
    private app: AppComponent,
    private searchBook: SearchBookComponent,
   ) { }
  

  ngOnInit() {
    if (this.app.IsExpired("r"))
      return;
    if (localStorage.getItem("user_type")=="r")
    this.getSuggestion();
  }

  getSuggestion() {
    if (this.app.IsExpired("l,r"))
      return;
    this.user_type = localStorage.getItem("user_type");
    this.user_id = localStorage.getItem("user_id");
    this.bookService.GetSuggestion(this.user_id).subscribe((suggestion: any[]) => this.suggestion = suggestion,
      response => { alert((<any>response).error.alert) });
  }

  SearchBook(title, author_fullname) {
    localStorage.setItem("title", title);
    localStorage.setItem("author_fullname", author_fullname)
    this.app.RouteTo("app-search-book");
    this.searchBook.Redirect();
  }

  ChooseCover() {
    this.cover = Math.floor(Math.random() * 2 + 1)
    switch (this.cover) {
      case 1:
        return "tile1";
      case 2:
        return "tile2";
      case 3:
        return "tile3";
    }
    return "tile1";
  }
}
