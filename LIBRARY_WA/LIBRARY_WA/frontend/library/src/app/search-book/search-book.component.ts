import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { Http } from '@angular/http';
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

  bookData: Book[] = []; // [{ book_id: 'test', title: 'test', ISBN: 'test', author_fullname: 'test', year: 'test', language: 'test',  type: 'test',description: 'test'}];
  public author = [];
  public bookType: Book[] = [{ book_id: 'test', title: 'test', ISBN: 'test', author_fullname: 'test', year: 'test', language: 'test', type: 'test', description: 'test', is_available: true }, { book_id: 'test', title: 'test', ISBN: 'test', author_fullname: 'test', year: 'test', language: 'test', type: 'test2', description: 'test', is_available: true }];

  public language = [];
  public values =[];
  searchBookForm: FormGroup;
  column: String[] = ["Id. książki", "ISBN", "Tytuł", "Autor", "Rok wydania", "Język wydania", "Rodzaj"];
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

    //pobierz wszystkie typy książki
    //pobierz wszystkie języki
   // this.resultData=[{ BookId: 'test', title: 'test', ISBN: 'test', authorFullName: 'test', releaseDate: 'test', year: 'test', language: 'test'  type: 'test' }];
    this.GetBookType();
    this.GetAuthor();
    this.GetLanguage();

    var names = this.column;
    this.searchBookForm = this.formBuilder.group({
    book_id:'',
    ISBN: [''],
    title: '',
    author_fullname: [''],
    year: [''],
    language: '',
    type: '',
  });
  }


  SearchBook() {
    var searchForm = this.book;
    for (const field in this.searchBookForm.controls) {
      if (field.toString().replace(" ", "").length == 0) {
        this.values.push('%');
    }
      else {
      this.values.push("%"+field.toString()+"%");
    }
    }
 
 
    return this.bookService.SearchBook().pipe(
      map((res: Book[]) => res.filter(book => (book.book_id == this.values[0] && book.ISBN == this.values[1]
        && book.title == this.values[2] && book.author_fullname == this.values[3] && book.year == this.values[4]
        && book.language == this.values[5] && book.type == this.values[6]))))
      .toPromise()
      .then(response => this.bookData = response);
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
}
//tu tez przekazac zmienna czy jest ktos zalogowany
