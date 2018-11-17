import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable()
export class UserService {
  private headers: HttpHeaders;
  private accessPointUrl: string = 'https://localhost:5001/api/user';

  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8'});
  
  }

  public isLogged(user) {
    
    // Get all jogging data
    return this.http.post(this.accessPointUrl + '/IsLogged',user, { headers: this.headers });//, withCredentials: true });
  }

  public get() {
    // Get all jogging data
    return this.http.get(this.accessPointUrl, { headers: this.headers, withCredentials: true });
  }

  public addUser(user) {
    return this.http.post(this.accessPointUrl, user, { headers: this.headers, withCredentials: true });
  }

  public ifUserExists(user) {
    // Get all jogging data
    var result = ["", ""];
    this.http.get(this.accessPointUrl + '/IfEmailxists/' + user.email, { headers: this.headers, withCredentials: true }).subscribe(result => result[0] = result.toString());

    this.http.get(this.accessPointUrl + '/IfLoginExists/' + user.login, { headers: this.headers, withCredentials: true }).subscribe(result => result[1] = result.toString());
    return result;
  }

  public remove(payload) {
    return this.http.delete(this.accessPointUrl + '/' + payload.id, { headers: this.headers, withCredentials: true });
  }

  public update(payload) {
    return this.http.put(this.accessPointUrl + '/' + payload.id, payload, { headers: this.headers, withCredentials: true });
  }

 // public login(user) {
   // return this.http.get(this.accessPointUrl + '/login', user, { headers: this.headers });
  //}
}
