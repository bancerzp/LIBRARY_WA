import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { BookService } from '../_services/book.service';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { map } from 'rxjs/operators';
import { Book } from '../_models/book';
import { Router } from '@angular/router';
import { ComboBoxComponent } from 'ng2-combobox';
import { AppComponent } from '../app.component';
@Component({
  selector: 'app-add-book',
  templateUrl: './add-book.component.html',
  styleUrls: ['./add-book.component.css'],
  providers: [BookService, AppComponent]
})
export class AddBookComponent implements OnInit {
  addBookForm: FormGroup;
  id: number;
  column: String[] = ["Id. książki", "ISBN", "Tytuł", "Autor", "Rok wydania", "Język wydania", "Rodzaj"];
  public author=[];
  public bookType = [];
  public language = [];

  book: Book;
  submitted: bool;
  message: String;

  constructor(private formBuilder: FormBuilder,
    private http: Http,
    private bookService: BookService,
    private app: AppComponent) { }

  ngOnInit() {
    if (this.app.IsExpired("l"))
      return;
    this.submitted = false;
    this.message = "";
    this.GetBookType();  
    this.GetAuthor();    
    this.GetLanguage();
    this.book = new Book(null, "", "", "", "", "", "", "", true);
    this.addBookForm = this.formBuilder.group({
      isbn: ['', [Validators.pattern("[0-9]{13}")]],
      //  Validators.required], this.CheckISBNExistsInDB.bind(this)],
      title: ['', [Validators.required, Validators.maxLength(50)]],
      authorFullname: ['', [Validators.required, Validators.maxLength(100)]],
      year: [''],//, [Validators.required, Validators.pattern("[1-9][0-9]{3}")]],
      language:['', [Validators.required, Validators.maxLength(20)]],
      type: ['', [Validators.required, Validators.maxLength(30)]],
      description: ['', [Validators.maxLength(300)]],
      isAvailable:true,
    });
  }

  AddBook() {
    if (this.app.IsExpired("l"))
      return;
    var m = this.book;
    m.isAvailable = true;
  
    this.id = 0;
    this.submitted = false;
    var added=this.bookService.AddBook(m).subscribe(
      (data: Book) => { this.id = Number(data.bookId); this.message = "Książka dodana poprawnie " + this.id; this.clearForm();},
      response => { this.message = (<any>response).error.alert });
    this.submitted = true;
    }

  clearForm() {
    Object.keys(this.addBookForm.controls).forEach((name) => {
      this.addBookForm.controls[name].setValue("");
      this.addBookForm.controls[name].reset();
    });
    
  }

  CheckISBNExistsInDB(control: FormControl) {
    return this.bookService.IfISBNExists(control.value).pipe(
      map(((res: any[]) => res.filter(book => book.ISBN == control.value).length > 0 ? { 'ISBNTaken': false } : null)));
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
