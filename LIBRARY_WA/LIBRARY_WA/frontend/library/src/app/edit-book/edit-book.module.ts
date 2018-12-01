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
  public displayVolume: boolean;


  constructor(private formBuilder: FormBuilder,
  private http: Http,
  private bookService: BookService,
  private route: ActivatedRoute,
  private app: AppComponent) { }
  submitted: boolean;
  editBookForm: FormGroup;


  ngOnInit() {
    this.displayVolume = true;
    
    //this.route.queryParams
    //  .subscribe(params => {
       
    //    alert(params.book_id.value());
    //    this.book.book_id = params.book_id;
    //  }
    //  )
    
    this.GetBookById();
    this.GetVolumeByBookId();

    this.submitted = true;
    this.GetBookType();
    this.GetAuthor();
    this.GetLanguage();

    this.editBookForm = this.formBuilder.group({
      book_id: [this.book.book_id],
      isbn: [this.book.isbn, [Validators.pattern("[0-9]{13}"),
      Validators.required], this.CheckISBNExistsInDB.bind(this)],
      title: [this.book.title, [Validators.required, Validators.maxLength(50)]],
      author_fullname: [this.book.author_fullname, [Validators.required, Validators.maxLength(100)]],
      year: [this.book.year, [Validators.pattern("[1-9][0-9]{3}"), Validators.required]],
      language: [this.book.language, [Validators.required, Validators.maxLength(20)]],
      type: [this.book.type, [Validators.required, Validators.maxLength(30)]],
      description: [this.book.description, [Validators.maxLength(300)]],
      is_available: true,
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

  GetVolumeByBookId() {
    return this.bookService.GetVolumeByBookId(localStorage.getItem("book_id")).subscribe((volumes: any[]) => this.volume = volumes);
  }

  RemoveVolume(id) {
    if (this.app.IsExpired())
      return;
  this.displayVolume = false;
    this.bookService.RemoveVolume(id).subscribe(data => {
      alert("Egzemplarz został poprawnie usunięty");
      this.volume = this.volume.filter(volume => volume.volume_id != id);
    },
      Error => { alert("Błąd dodawania książki") });
    
    this.displayVolume = true;
}
  EditBook() {
    if (this.app.IsExpired())
      return;
    this.bookService.EditBook(this.book).subscribe(data => {
      alert("Egzemplarz został poprawnie usunięty");
     
    });

  }

  GetBookById() {
    if (this.app.IsExpired())
      return;
    return this.bookService.GetBookById(localStorage.getItem("book_id")).subscribe((book: Book) => this.book = book);
  }

}
