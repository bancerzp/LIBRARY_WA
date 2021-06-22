import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { BookService } from '../_services/book.service';
import { map } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { Book } from '../_models/book';
import { Volume } from '../_models/Volume';
import { AppComponent } from '../app.component';

@Component({
  selector: 'app-edit-book',
  templateUrl: './edit-book.component.html',
  providers: [BookService, AppComponent]
})

@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ]
})
export class EditBookComponent {
  public author = [];
  public bookType = [];
  public language = [];
  book:Book = new Book(null,null, "", "", "", "", "", "",  true);
  public volume: Volume[] = [];
  public displayVolume: bool;
  public message: String;
  public isbn: String;


  constructor(private formBuilder: FormBuilder,
  private http: Http,
  private bookService: BookService,
  private route: ActivatedRoute,
  private app: AppComponent) { }
  submitted: bool;
  editBookForm: FormGroup;


  ngOnInit() {
    if (this.app.IsExpired("l"))
      return;
    this.displayVolume = true;

    this.GetBookById();
    this.GetVolumeByBookId();

    this.submitted = true;
    this.GetBookType();
    this.GetAuthor();
    this.GetLanguage();
    this.isbn = this.book.isbn;
    this.editBookForm = this.formBuilder.group({
      bookId: [this.book.bookId],
      isbn: [this.book.isbn, [Validators.pattern("[0-9]{13}"),
      Validators.required], this.CheckISBNExistsInDB.bind(this)],
      title: [this.book.title, [Validators.required, Validators.maxLength(50)]],
      authorFullname: [this.book.authorFullname, [Validators.required, Validators.maxLength(100)]],
      year: [this.book.year, [Validators.pattern("[1-9][0-9]{3}"), Validators.required]],
      language: [this.book.language, [Validators.required, Validators.maxLength(20)]],
      type: [this.book.type, [Validators.required, Validators.maxLength(30)]],
      description: [this.book.description, [Validators.maxLength(300)]],
      isAvailable: [true],
    });
  }

  CheckISBNExistsInDB(control: FormControl) {
    return this.bookService.IfISBNExists(control.value).pipe(
      map(((res: bool) => (res == true && this.book.isbn == this.isbn) ? { 'ISBNTaken': false } : null)));
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

  GetVolumeByBookId() {
    return this.bookService.GetVolumeByBookId(localStorage.getItem("bookId")).subscribe((volumes: Volume[]) => { this.volume = volumes });
  }

  RemoveVolume(id) {
    if (this.app.IsExpired("l"))
      return;
    this.submitted = false;
    this.displayVolume = false;
    this.bookService.RemoveVolume(id).subscribe(data => {
    this.message="Egzemplarz został poprawnie usunięty";
      this.volume = this.volume.filter(volume => volume.volumeId != id);
    },
      response => { this.message = (<any>response).error.alert});
    this.displayVolume = true;
    this.submitted = true;
}
  UpdateBook() {
    if (this.app.IsExpired("l"))
      return;
    this.bookService.UpdateBook(this.book).subscribe(data => {
      this.message=("Książki została poprawnie zapisana");
    }, response => { this.message = (<any>response).error });

  }

  GetBookById() {
    if (this.app.IsExpired("l"))
      return;
    return this.bookService.GetBookById(localStorage.getItem("bookId")).subscribe(
      (bookGet: Book) => this.book = bookGet, response => { this.message = (<any>response).error.alert });
  }

}
