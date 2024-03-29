import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Book } from '../_models/book';
import { Volume } from '../_models/Volume';

@Injectable()
export class BookService {
   
  private headers: HttpHeaders;
  private accessPointUrl: string = 'https://localhost:5001/api/book';
  public books: Book[];
 
  readBooks(Book): Observable<any> {
    return this.http.get(this.accessPointUrl, { headers: this.headers });
  }

  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token")  });
  }
  
  public IfISBNExists(ISBN) {
    return this.http.get(this.accessPointUrl + '/IfISBNExists/' + ISBN, { headers: this.headers });
  }

  //Book function
  public AddBook(book) {
    return this.http.post(this.accessPointUrl + "/AddBook", book, { headers: this.headers });
  }

  public GetBookById(bookId) {
    return this.http.get(this.accessPointUrl + "/GetBookById/" + bookId, { headers: this.headers });
  }

  public UpdateBook(book) {
    return this.http.put(this.accessPointUrl +"/UpdateBook", book, { headers: this.headers });
  }

  public SearchBook(book) {
    return this.http.post(this.accessPointUrl + "/SearchBook", book, { headers: this.headers });
  }

  public RemoveBook(id) {
    return this.http.delete(this.accessPointUrl + "/RemoveBook/" + id, { headers: this.headers });
  }

  //Volume function
  public AddVolume(id) {
    return this.http.post(this.accessPointUrl + "/AddVolume/" , id, { headers: this.headers });
  }

  public GetVolume() {
    return this.http.post(this.accessPointUrl + "/GetVolume", { headers: this.headers });
  }

  public GetVolumeByBookId(id) {
    return this.http.get(this.accessPointUrl + "/GetVolumeByBookId/"+id, { headers: this.headers });
  }
  public RemoveVolume(id) {
  return this.http.delete(this.accessPointUrl + "/RemoveVolume/" + id, { headers: this.headers });
  }

  public GetSuggestion(userId) {
    return this.http.get(this.accessPointUrl + "/GetSuggestion/"+userId,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });

  }
}
