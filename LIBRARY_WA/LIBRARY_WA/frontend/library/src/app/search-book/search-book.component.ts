import { BookService } from '../_services/book.service';
import { Book } from '../_models/book';
import { DictionaryService } from '../_services/dictionary.service';
import { Volume } from '../_models/Volume';
import { AppComponent } from '../app.component';
import { Http } from '@angular/http';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
@Component({
  selector: 'app-search-book',
  templateUrl: './search-book.component.html',
  styleUrls: ['./search-book.component.css'],
  providers: [BookService, AppComponent, SearchBookComponent, DictionaryService]
})
export class SearchBookComponent implements OnInit {
  @Output() book = new EventEmitter<Book>();

  submitted: Boolean;
  userType: string;
  title: String;
  authorFullname: String;
  bookData: Book[] = [];
  public author = [];
  public bookType = [];
  public language = [];
  public values = [];
  public userAction = [];
  searchBookForm: FormGroup;
  column: String[] = ["Id. książki", "Tytuł", "ISBN", "Autor", "Rok wydania", "Język wydania"];
  userId: String;
  message: String

  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private bookService: BookService,
    private app: AppComponent,
    private dictionaryService: DictionaryService
  ) {
  }

  ngOnInit() {
    this.userType = localStorage.getItem("userType");

    this.submitted = false;
    //pobranie wartości do list rozwijanych
    this.GetBookType();
    this.GetAuthor();
    this.GetLanguage();
    
    this.searchBookForm = this.formBuilder.group({
      bookId: '',
      isbn: [''],
      title: '',
      authorFullname: [''],
      year: [''],
      language: '',
      type: ''
    });
    if (localStorage.getItem("title") != null) {
      this.searchBookForm.controls['title'].setValue(localStorage.getItem("title"));
      this.searchBookForm.controls['authorFullname'].setValue(localStorage.getItem("authorFullname"));
      this.SearchBook();
    }
  }

  SearchBook() {
    this.message = "";
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
    return this.dictionaryService.GetAuthor().subscribe((authors: Array<any>) => this.author = authors);
  }

  GetBookType() {
    return this.dictionaryService.GetBookType().subscribe((types: Array<any>) => this.bookType = types);
  };

  GetLanguage() {
    return this.dictionaryService.GetLanguage().subscribe((languages: Array<any>) => this.language = languages);
  };

  EditBook(bookId) {
    this.message = "";
    if (this.app.IsExpired("l"))
      return;
    localStorage.setItem("bookId", bookId);
    this.app.RouteTo("app-edit-book");
  }

  RemoveBook(id) {
    this.message = "";
    if (this.app.IsExpired("l"))
      return;

    this.submitted = false;
    this.bookService.RemoveBook(id).subscribe(data => {
      this.message = "Książka została poprawnie usunięta.";
      this.bookData = this.bookData.filter(book => book.bookId != id);
    },
      response => { this.message = (<any>response).error.alert });
   
    this.submitted = true;
  }

  AddVolume(id) {
    this.message = "";
    if (this.app.IsExpired("l"))
      return;
    this.submitted = false;
    this.bookService.AddVolume(id).subscribe(
      (volume: Volume) => {
      this.message = "Egzemplarz dodany poprawnie: " + volume.volumeId
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
