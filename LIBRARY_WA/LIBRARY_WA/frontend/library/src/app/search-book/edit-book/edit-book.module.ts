import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { BookService } from '../../_services/book.service';
import { map } from 'rxjs/operators';
import { ActivatedRoute } from '@angular/router';
import { Book } from '../../_models/book';
import { Volume } from '../../_models/Volume';

@Component({
  selector: 'app-edit-book',
  templateUrl: './edit-book.component.html',
  providers: [BookService]
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
  public book: Book;
  public volume: Volume[]=[];


  constructor(private formBuilder: FormBuilder,
    private http: Http,
    private bookService: BookService,
    private route: ActivatedRoute) { }
  submitted: boolean;
  updateBookForm: FormGroup;


  ngOnInit() {
    this.route.params.subscribe(params => {
      this.book.book_id = +params['book_id']; // (+) converts string 'id' to a number

    });
    this.GetBookById(this.book.book_id);

    this.submitted = false;
    this.GetBookType();
    this.GetAuthor();
    this.GetLanguage();

    this.updateBookForm = this.formBuilder.group({
      book_id: [this.book.isbn],
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

  GetVolumes() {

  }

  //error
  GetBookById(id) {
    this.bookService.AddVolume(id).subscribe((book:Book) => this.book=book);
  }
}
