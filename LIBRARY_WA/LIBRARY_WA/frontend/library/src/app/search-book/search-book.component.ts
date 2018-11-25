import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { Http, Response } from '@angular/http';
import { BookService } from '../_services/book.service';
import { Book } from '../_models/book';
import { map } from 'rxjs/operators';
import { forEach } from '@angular/router/src/utils/collection';
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
  bookData: Book[] = []; // [{ book_id: 'test', title: 'test', ISBN: 'test', author_fullname: 'test', year: 'test', language: 'test',  type: 'test',description: 'test'}];
  public author = [];
  public bookType = [];
  public language = [];
  public values = [];
  public action = [];
  searchBookForm: FormGroup;
  column: String[] = ["Id. książki", "Tytuł", "ISBN",  "Autor", "Rok wydania", "Język wydania", "Rodzaj","Akcje"];
  columnAddReader:String[] =["Zarezerwuj"]
  columnAddLibrarian: String[] = ["Zarezerwuj","Edytuj","Usuń","Dodaj egzemplarz"]


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
  areBooks() {
    return this.bookData.length != 0;
  }

  //ngOnInit() {
  //  this.BookService.readProducts()
  //    .subscribe(products =>
  //      this.bookData = products['records']
  //    );
 // }
 

  ngOnInit() {
   // this.recordDeleted.emit("Wydarzenie wyemitowane");
    //w zależności od typu użytkownika różne akcje
    this.action = this.columnAddLibrarian;
    this.submitted = false;
    //pobranie wartości do list rozwijanych
    this.GetBookType();
    this.GetAuthor();
    this.GetLanguage();

    var names = this.column;
    this.searchBookForm = this.formBuilder.group({
    book_id:'1',
    ISBN: [''],
    title: '',
    author_fullname: [''],
    year: [''],
    language: '',
    type: '',
    });
    this.SearchBook();
  }

  SearchBook() {
    this.submitted = true;
   
    Object.keys(this.searchBookForm.controls).forEach((name) => {
      var s = this.searchBookForm.controls[name].value;
      if (s.replace(" ", "").replace("'","").length == 0) {
        this.values.push('%');
      }
      else {
        this.values.push(s.replace("'",""));
      }
    });

    return this.bookService.SearchBook(this.values).subscribe((data:Book[]) => this.bookData=data)
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


  UpdateBook() {

  }

  DeleteBook() {

  }

  AddVolume() {

  }

  readOneProduct(id) { }


  rentBook(bookId,userId) {

  }
}
//tu tez przekazac zmienna czy jest ktos zalogowany
