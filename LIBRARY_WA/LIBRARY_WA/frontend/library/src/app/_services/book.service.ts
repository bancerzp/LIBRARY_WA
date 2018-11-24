import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Book } from '../_models/book';

@Injectable()
export class BookService {
   
  private headers: HttpHeaders;
  private accessPointUrl: string = 'https://localhost:5001/api/book';
  public books: Book[];
 
  readBooks(Book): Observable<any> {
    return this.http.get(this.accessPointUrl, { headers: this.headers });
  }

  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8' });
  }

  public GetAuthor() {
    return this.http.get(this.accessPointUrl + "/GetAuthor", { headers: this.headers });
  }
  
  public GetBookType() {

    return this.http.get(this.accessPointUrl + "/GetBookType", { headers: this.headers });
  }

  public GetLanguage() {
    return this.http.get(this.accessPointUrl + "/GetLanguage", { headers: this.headers });
  }
  
  public SearchBook(book) {

    return this.http.get(this.accessPointUrl + "/SearchBook", { headers: this.headers, params:book});
  }
  
  public IfISBNExists(ISBN) {
    return this.http.get(this.accessPointUrl + '/IfISBNExists/' + ISBN, { headers: this.headers });
  }
 
  public AddBook(book) {
    return this.http.post(this.accessPointUrl + "/AddBook", book, { headers: this.headers });
  }

  public AddVolume(id) {
    return this.http.post(this.accessPointUrl + "/AddVolume"+id, { headers: this.headers });
  }


  public remove(payload) {
    return this.http.delete(this.accessPointUrl + '/' + payload.id, { headers: this.headers });
  }

  public update(payload) {
    return this.http.put(this.accessPointUrl + '/' + payload.id, payload, { headers: this.headers });
  }
}
