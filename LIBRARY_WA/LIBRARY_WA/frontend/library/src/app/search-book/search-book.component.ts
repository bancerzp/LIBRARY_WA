import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import { Validators, FormBuilder, FormGroup, FormsModule } from '@angular/forms';
import { Http, Response } from '@angular/http';
import { BookService } from '../_services/book.service';
import { Book } from '../_models/book';
import { map } from 'rxjs/operators';
import { forEach } from '@angular/router/src/utils/collection';
import { Volume } from '../_models/Volume';
@Component({
  selector: 'app-search-book',
  templateUrl: './search-book.component.html',
  styleUrls: ['./search-book.component.css'],
  providers: [BookService]
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
  column: String[] = ["Id. książki", "Tytuł", "ISBN", "Autor", "Rok wydania", "Język wydania", "Rodzaj", "Akcje"];
  columnAddReader: UserAction[] = [ new UserAction("Zarezerwuj","Reserve")]
  columnAddLibrarian: UserAction[] = [new UserAction("Zarezerwuj", "Reserve()"), new UserAction("Edytuj", "Update()"), new UserAction("Usuń", "Delete()"), new UserAction("Dodaj egzemplarz", "AddVolume()")]
 
  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private bookService: BookService,
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
    //wypisywanie błędu
    this.bookService.ReserveBook(book_id, user_id).subscribe();
  }

  ReserveBookReader(book_id) {
    this.bookService.ReserveBook(book_id, localStorage.getItem("user_id")).subscribe();
  }

  //nie zrobione
  UpdateBook() {
  //  this.bookService.update(book_id, localStorage.getItem("user_id")).subscribe();
  }

  RemoveBook(id) {
    this.submitted = false;
    this.bookService.RemoveBook(id).subscribe(this.SearchBook);
    this.bookData.splice(this.bookData.indexOf(this.bookData.find(book => book.book_id = id)), 1);
 //   this.SearchBook();
    this.submitted = true;
  }

  AddVolume(id) {
    this.bookService.AddVolume(id).subscribe(
      (volume: Volume) => { alert("Egzemplarz dodany poprawnie: "+volume.volume_id) },
      Error => { alert("Błąd dodawania książki" ) });
  }
  

  rentBook(bookId,userId) {

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
