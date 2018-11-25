import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { BookService } from '../_services/book.service';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { map } from 'rxjs/operators';
import { Book } from '../_models/book';
import { Router } from '@angular/router';
import { ComboBoxComponent } from 'ng2-combobox';
@Component({
  selector: 'app-add-book',
  templateUrl: './add-book.component.html',
  styleUrls: ['./add-book.component.css'],
  providers: [BookService]
})
export class AddBookComponent implements OnInit {
  addBookForm: FormGroup;
  id: number;
  column: String[] = ["Id. książki", "ISBN", "Tytuł", "Autor", "Rok wydania", "Język wydania", "Rodzaj"];
  public author=[];
  public bookType = [];
  public language = [];
  @Output() book = new EventEmitter<Book>();
  submitted: boolean;

  constructor(private formBuilder: FormBuilder,
    private http: Http,
    private bookService: BookService   ) { }

  ngOnInit() {
    this.submitted = false;
  
    this.GetBookType();  
    this.GetAuthor();    
    this.GetLanguage();

    this.addBookForm = this.formBuilder.group({
      ISBN: ['', [Validators.pattern("[0-9]{13}"),
        Validators.required], this.CheckISBNExistsInDB.bind(this)],
      title: ['', [Validators.required, Validators.maxLength(50)]],
      author_fullname: ['', [Validators.required, Validators.maxLength(100)]],
      year: ['', [Validators.pattern("[1-9][0-9]{3}"),Validators.required]],
      language: [Validators.required, Validators.maxLength(20)],
      type: ['', [Validators.required, Validators.maxLength(30)]],
      description: ['', [Validators.maxLength(300)]],
      is_available:true,
    });
  }

  AddBook() {
    var m = this.book;
    this.id = 0;
    var added=this.bookService.AddBook(m).subscribe(
      (data: Book) => { this.id = Number(data.book_id), alert("Książka dodana poprawnie " +this.id)},
      Error => { alert("Błąd dodawania książki") });


      //   this.user = this.addUserForm.value();
      // this.userService.addUser(m);
      //    .subscribe(user =>
      //    this.user = user['records']);
      this.submitted = true;
    }

  ClearForm() { this.book = new EventEmitter<Book>();}

  CheckISBNExistsInDB(control: FormControl) {
    return this.bookService.IfISBNExists(control.value).pipe(
      map(((res: any[]) => res.filter(book => book.ISBN == control.value).length == 0 ? { 'ISBNTaken': false } : { 'ISBNTaken': true })));
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
 
}
