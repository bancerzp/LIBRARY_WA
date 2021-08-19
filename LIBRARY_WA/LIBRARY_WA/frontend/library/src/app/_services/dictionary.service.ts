import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class DictionaryService {
   
  private headers: HttpHeaders;
  private accessPointUrl: string = 'https://localhost:5001/api/dictionary';
 
  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8'});
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
}
