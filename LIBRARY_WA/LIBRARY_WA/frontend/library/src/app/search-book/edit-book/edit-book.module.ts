import { NgModule, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { BookService } from '../../_services/book.service';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-edit-book',
  templateUrl: './app-edit-book.component.html',
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


  constructor(private formBuilder: FormBuilder,
    private http: Http,
    private bookService: BookService) { }
  submitted: boolean;
  addBookForm: FormGroup;
  ngOnInit() {
  this.submitted = false;

this.GetBookType();
this.GetAuthor();
this.GetLanguage();

this.addBookForm = this.formBuilder.group({
  isbn: ['', [Validators.pattern("[0-9]{13}"),
  Validators.required], this.CheckISBNExistsInDB.bind(this)],
  title: ['', [Validators.required, Validators.maxLength(50)]],
  author_fullname: ['', [Validators.required, Validators.maxLength(100)]],
  year: ['', [Validators.pattern("[1-9][0-9]{3}"), Validators.required]],
  language: [Validators.required, Validators.maxLength(20)],
  type: ['', [Validators.required, Validators.maxLength(30)]],
  description: ['', [Validators.maxLength(300)]],
  is_available: true,
});}


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
