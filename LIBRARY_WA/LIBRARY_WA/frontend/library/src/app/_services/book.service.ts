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
  
  //get data to combobox
  public GetAuthor() {
    return this.http.get(this.accessPointUrl + "/GetAuthor", { headers: this.headers });
  }
  
  public GetBookType() {
    return this.http.get(this.accessPointUrl + "/GetBookType", { headers: this.headers });
  }

  public GetLanguage() {
    return this.http.get(this.accessPointUrl + "/GetLanguage", { headers: this.headers });
  }

  public IfISBNExists(ISBN) {
    return this.http.get(this.accessPointUrl + '/IfISBNExists/' + ISBN, { headers: this.headers });
  }

  //Book function

  public AddBook(book) {
    return this.http.post(this.accessPointUrl + "/AddBook", book,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) } );
  }

  public GetBookById(book_id) {
    return this.http.get(this.accessPointUrl + "/GetBookById/" + book_id, { headers: this.headers });
  }

  public UpdateBook(book) {
    return this.http.put(this.accessPointUrl +"/UpdateBook", book,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public SearchBook(book) {
    return this.http.post(this.accessPointUrl + "/SearchBook", book,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public RemoveBook(id) {
    return this.http.delete(this.accessPointUrl + "/RemoveBook/" + id,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  //Volume function
  public AddVolume(id) {
    return this.http.post(this.accessPointUrl + "/AddVolume/" , id,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public GetVolume() {
    return this.http.post(this.accessPointUrl + "/GetVolume",
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public GetVolumeByBookId(id) {
    return this.http.get(this.accessPointUrl + "/GetVolumeByBookId/"+id,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }
  public RemoveVolume(id) {
  return this.http.delete(this.accessPointUrl + "/RemoveVolume/" + id,
    { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }
  
    //Rezerwacje
  public ReserveBook(book_id, user_id) {
    var values = [book_id, user_id]
    return this.http.put(this.accessPointUrl + "/ReserveBook", values,
    { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public RentBook(reservation_id) {
    var values = [reservation_id]
    return this.http.put(this.accessPointUrl + "/RentBook",values,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public ReturnBook(rent_id) {
    var values = [rent_id];
    return this.http.post(this.accessPointUrl + "/ReturnBook",values,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public GetSuggestion(user_id) {
    return this.http.get(this.accessPointUrl + "/GetSuggestion/"+user_id,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });

  }
  
}
