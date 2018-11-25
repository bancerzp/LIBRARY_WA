import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable()
export class UserService {
  private headers: HttpHeaders;
  private accessPointUrl: string = 'https://localhost:5001/api/user';

  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8'});
  }

  public IsLogged(user) {
    return this.http.post(this.accessPointUrl + '/IsLogged',user, { headers: this.headers });//, withCredentials: true });
  }

  public get() {
    return this.http.get(this.accessPointUrl, { headers: this.headers, withCredentials: true });
  }

  public AddUser(user) {
    return this.http.post(this.accessPointUrl +'/AddUser', user, { headers: this.headers});
  }

  public IfLoginExists(login) {
    return this.http.get(this.accessPointUrl + '/IfLoginExists/' + login, { headers: this.headers });
  }

  public IfEmailExists(email) {
    return this.http.get(this.accessPointUrl + '/IfEmailExists/' + email, { headers: this.headers});
  }

  public SearchUser(user) {

    return this.http.get(this.accessPointUrl + "/SearchBook", { headers: this.headers, params: user });
  }

  
  //-------------------------userdata
  public GetUser(id) {
    return this.http.get(this.accessPointUrl + "/GetUser/"+id, { headers: this.headers });
  }
  public GetRent(id) {
    return this.http.get(this.accessPointUrl + "/GetRent/" + id, { headers: this.headers });
  }
  public GetReservation(id) {
    return this.http.get(this.accessPointUrl + "/GetReservation/" + id, { headers: this.headers });
  }

  public GetRenth(id) {
    return this.http.get(this.accessPointUrl + "/GetRenth/" + id, { headers: this.headers });
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
