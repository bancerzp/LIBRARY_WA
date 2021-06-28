import {
  Component, OnInit
} from '@angular/core';
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
  userType: String;
  userId: String;
  cover: number;
  classs: string;

  constructor(private bookService: BookService,
    private app: AppComponent,
    private searchBook: SearchBookComponent,
   ) { }
  

  ngOnInit() {
    this.classs = "tile1";
    if (this.app.IsExpired("r"))
      return;
    if (localStorage.getItem("userType")=="r")
    this.getSuggestion();
  }

  getSuggestion() {
    if (this.app.IsExpired("l,r"))
      return;
    this.userType = localStorage.getItem("userType");
    this.userId = localStorage.getItem("userId");
    this.bookService.GetSuggestion(this.userId).subscribe((suggestion: any[]) => this.suggestion = suggestion,
      response => { alert((<any>response).error.alert) });
  }

  SearchBook(title, authorFullname) {
    localStorage.setItem("title", title);
    localStorage.setItem("authorFullname", authorFullname)
    this.app.RouteTo("app-search-book");
    this.searchBook.Redirect();
  }

  ChooseCover() {
    this.cover = Math.floor(Math.random() * 2 + 1)
    switch (this.cover) {
      case 1:
        this.classs="tile1";
        return "tile1";
      case 2:
        this.classs = "tile2";
        return "tile2";
      case 3:
        this.classs = "tile3";
        return "tile3";
    }
    return "tile1";
  }
}
