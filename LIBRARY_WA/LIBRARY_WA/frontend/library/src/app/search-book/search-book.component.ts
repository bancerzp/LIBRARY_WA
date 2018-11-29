import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import { Validators, FormBuilder, FormGroup, FormsModule } from '@angular/forms';
import { Http, Response } from '@angular/http';
import { BookService } from '../_services/book.service';
import { Book } from '../_models/book';
import { map } from 'rxjs/operators';
import { forEach } from '@angular/router/src/utils/collection';
import { Volume } from '../_models/Volume';
import { AppComponent } from '../app.component';
@Component({
  selector: 'app-search-book',
  templateUrl: './search-book.component.html',
  styleUrls: ['./search-book.component.css'],
  providers: [BookService, AppComponent]
})
export class SearchBookComponent implements OnInit {
  @Output() book = new EventEmitter<Book>();
  @Output() recordDeleted = new EventEmitter<any>();
  @Output() newClicked = new EventEmitter<any>();
  @Output() editClicked = new EventEmitter<any>();
  //@Input()

  submitted: boolean;
  userType: string;
  bookData: Book[] = []; // [{ book_id: 'test', title: 'test', ISBN: 'test', author_fullname: 'test', year: 'test', language: 'test',  type: 'test',description: 'test'}];
  public author = [];
  public bookType = [];
  public language = [];
  public values = [];
  public userAction = [];
  searchBookForm: FormGroup;
  column: String[] = ["Id. książki", "Tytuł", "ISBN", "Autor", "Rok wydania", "Język wydania", "Rodzaj"];
  columnAddReader: UserAction[] = [ new UserAction("Zarezerwuj","Reserve")]
  columnAddLibrarian: UserAction[] = [new UserAction("Zarezerwuj", "Reserve()"), new UserAction("Edytuj", "Update()"), new UserAction("Usuń", "Delete()"), new UserAction("Dodaj egzemplarz", "AddVolume()")]
  user_id: String;
  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private bookService: BookService,
    private app: AppComponent,
  ) {
   // bookService.get().subscribe((data: any) => this.resultData = data);
  }
 /*
  public deleteRecord(record) {
    this.recordDeleted.emit(record);
  }

  public editRecord(record) {
    const clonedRecord = Object.assign({}, record);
    this.editClicked.emit(clonedRecord);
  }

  public newRecord() {
    this.newClicked.emit();
  }
  */

  ngOnInit() {
   // this.recordDeleted.emit("Wydarzenie wyemitowane");
    //w zależności od typu użytkownika różne akcje
    this.userType = localStorage.getItem("user_type");
   
    this.submitted = false;
    //pobranie wartości do list rozwijanych
    this.GetBookType();
    this.GetAuthor();
    this.GetLanguage();

    var names = this.column;
    this.searchBookForm = this.formBuilder.group({
    book_id:'',
    isbn: [''],
    title: '',
    author_fullname: [''],
    year: [''],
    language: '',
    type: ''
    });
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

    return this.bookService.SearchBook(this.values).subscribe((data: Book[]) => this.bookData = data)
    
  }

  clearForm() {
    this.searchBookForm.reset();
  }

  GetAuthor() {
    return this.bookService.GetAuthor().subscribe((authors: any[]) => this.author = authors);
  }

  GetBookType() {
    return this.bookService.GetBookType().subscribe((types: any[]) => this.bookType=types);
  };

  GetLanguage() {
    return this.bookService.GetLanguage().subscribe((languages: any[]) => this.language = languages);
  };

  ReserveBookLibrarian(book_id, user_id) {
    if (this.app.IsExpired())
      return;
    //wypisywanie błędu
    this.bookService.ReserveBook(book_id, Number.parseInt(user_id)).subscribe((res: Response) => {
      alert("Książka została zarezerwowana. Miejsce w kolejce: " + res.json)
    },
      (Error: Response) => { alert("Błąd dodawania książki " + Error.json) });
  }

  ReserveBookReader(book_id) {
    if (this.app.IsExpired())
      return;
    this.bookService.ReserveBook(book_id, Number.parseInt(localStorage.getItem("user_id"))).subscribe();
    this.app.RouteTo("/app-edit-book");
  }

  //nie zrobione
  EditBook(book_id) {
    if (this.app.IsExpired())
      return;
      localStorage.setItem("book_id", book_id);
  //  this.bookService.update(book_id, localStorage.getItem("user_id")).subscribe();
  }

  RemoveBook(id) {
    if (this.app.IsExpired())
      return;

    this.submitted = false;
    this.bookService.RemoveBook(id).subscribe(data => {
      alert("Książka została poprawnie usunięta");
      this.bookData = this.bookData.filter(book => book.book_id != id);
    },
      Error => { alert("Błąd usuwania książki") });
 //   this.bookData.splice(this.bookData.indexOf(this.bookData.find(book => book.book_id = id)), 1);
 //   this.SearchBook();
    this.submitted = true;
  }

  AddVolume(id) {
    if (this.app.IsExpired())
      return;

    this.bookService.AddVolume(id).subscribe(
      (volume: Volume) => { alert("Egzemplarz dodany poprawnie: "+volume.volume_id) },
      Error => { alert("Błąd dodawania książki" ) });
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
