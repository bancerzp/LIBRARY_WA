import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { Resource } from '../_models/resource';

@Injectable()
export class ResourceService {
  private headers: HttpHeaders;
  private accessPointUrl: string = 'https://localhost:5001/api/resource';
  public resources: Resource[];
 
  readResources(Resource): Observable<any> {
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
    return this.http.get(this.accessPointUrl + "/GetLanguageee", { headers: this.headers });
  }





  public add(payload) {
    return this.http.post(this.accessPointUrl, payload, { headers: this.headers });
  }

  public remove(payload) {
    return this.http.delete(this.accessPointUrl + '/' + payload.id, { headers: this.headers });
  }

  public update(payload) {
    return this.http.put(this.accessPointUrl + '/' + payload.id, payload, { headers: this.headers });
  }
}
