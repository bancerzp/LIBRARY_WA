import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import {  FormBuilder, FormGroup } from '@angular/forms';
import { Http, Response } from '@angular/http';
import { BookService } from '../_services/book.service';
import { Book } from '../_models/book';
import { Volume } from '../_models/Volume';
import { AppComponent } from '../app.component';
import { Reservation } from '../_models/reservation';
@Component({
  selector: 'app-search-book',
  templateUrl: './search-book.component.html',
  styleUrls: ['./search-book.component.css'],
  providers: [BookService, AppComponent, SearchBookComponent]
})
export class SearchBookComponent implements OnInit {
  @Output() book = new EventEmitter<Book>();

  submitted: boolean;
  userType: string;
  title: String;
  author_fullname: String;
  bookData: Book[] = [];
  public author = [];
  public bookType = [];
  public language = [];
  public values = [];
  public userAction = [];
  searchBookForm: FormGroup;
  column: String[] = ["Id. książki", "Tytuł", "ISBN", "Autor", "Rok wydania", "Język wydania", "Rodzaj"];
  columnAddReader: UserAction[] = [new UserAction("Zarezerwuj", "Reserve")]
  columnAddLibrarian: UserAction[] = [new UserAction("Zarezerwuj", "Reserve()"), new UserAction("Edytuj", "Update()"), new UserAction("Usuń", "Delete()"), new UserAction("Dodaj egzemplarz", "AddVolume()")]
  user_id: String;
  message: String;
  reservation: Reservation;

  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private bookService: BookService,
    private app: AppComponent,
  ) {
  }
  
  ngOnInit() {
    this.userType = localStorage.getItem("user_type");

    this.submitted = false;
    //pobranie wartości do list rozwijanych
    this.GetBookType();
    this.GetAuthor();
    this.GetLanguage();
    
    this.searchBookForm = this.formBuilder.group({
      book_id: '',
      isbn: [''],
      title: '',
      author_fullname: [''],
      year: [''],
      language: '',
      type: ''
    });
    if (localStorage.getItem("title") != null) {
      this.searchBookForm.controls['title'].setValue(localStorage.getItem("title"));
      this.searchBookForm.controls['author_fullname'].setValue(localStorage.getItem("author_fullname"));
      this.SearchBook();
    }
  }

  SearchBook() {
    this.submitted = false;
    this.values = [];

    Object.keys(this.searchBookForm.controls).forEach((name) => {
      var s = this.searchBookForm.controls[name].value;
      if (s.replace(" ", "").replace("'", "").length == 0) {
        this.values.push('%');
      }
      else {
        this.values.push(s.replace("'", ""));
      }

    });
    this.submitted = true;
     this.bookService.SearchBook(this.values).subscribe((data: Book[]) => { this.bookData = data },
      response => { this.message = (<any>response).error.alert });
   
  }

  clearForm() {
    Object.keys(this.searchBookForm.controls).forEach((name) => {
      this.searchBookForm.controls[name].setValue("");
    });

  }

  GetAuthor() {
    return this.bookService.GetAuthor().subscribe((authors: any[]) => this.author = authors);
  }

  GetBookType() {
    return this.bookService.GetBookType().subscribe((types: any[]) => this.bookType = types);
  };

  GetLanguage() {
    return this.bookService.GetLanguage().subscribe((languages: any[]) => this.language = languages);
  };

  ReserveBookLibrarian(book_id, user_id) {
    this.submitted = false;
    if (this.app.IsExpired("l"))
      return;
   
    this.bookService.ReserveBook(book_id, Number.parseInt(user_id)).subscribe((res: any) => {
      this.reservation = res;
      this.message = "Książka została zarezerwowana. Miejsce w kolejce: " + this.reservation.queue;
    },
      response => { this.message = (<any>response).error.alert });
    this.submitted = true;
  }

  ReserveBookReader(book_id) {
    this.submitted = false;
    if (this.app.IsExpired("r"))
      return;
    this.bookService.ReserveBook(book_id, this.app.GetUserId()).subscribe((res: Response) => {
      this.message = "Książka została zarezerwowana. Miejsce w kolejce: " + this.reservation.queue
    },
      response => { this.message = (<any>response).error.alert });
    this.submitted = true;
  }

  
  EditBook(book_id) {
    if (this.app.IsExpired("l"))
      return;
    localStorage.setItem("book_id", book_id);
    this.app.RouteTo("app-edit-book");
  }

  RemoveBook(id) {
    if (this.app.IsExpired("l"))
      return;

    this.submitted = false;
    this.bookService.RemoveBook(id).subscribe(data => {
      this.message = "Książka została poprawnie usunięta.";
      this.bookData = this.bookData.filter(book => book.book_id != id);
    },
      response => { this.message = (<any>response).error.alert });
   
    this.submitted = true;
  }

  AddVolume(id) {
    if (this.app.IsExpired("l"))
      return;
    this.submitted = false;
    this.bookService.AddVolume(id).subscribe(
      (volume: Volume) => {
      this.message = "Egzemplarz dodany poprawnie: " + volume.volume_id
  },
      response => { this.message = (<any>response).error });
  
  this.submitted = true;
  }

  Redirect() {
    this.ngOnInit();
  }
}

export class UserAction {
  text: string;
  action: string;

  constructor(text: string,
    action: string) {
    this.text = text;
    this.action = action;
  }

}
