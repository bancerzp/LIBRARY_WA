import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable()
export class UserService {
  private headers: HttpHeaders;
  private accessPointUrl: string = 'https://localhost:5001/api/user';

  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") });
  }

  public IsLogged(user) {
    return this.http.post(this.accessPointUrl + '/IsLogged', user, { headers: this.headers });/*.subscribe(response => {
      let token = (<any>response).token;
      let user_id = (<any>response).id;
      let fullname = (<any>response).fullname;
      let user_type = (<any>response).user_type
      localStorage.setItem("jwt", user_id);
      localStorage.setItem("user_id", user_id);
      localStorage.setItem("fullname", fullname);
      c.setItem("user_type", user_type);
      return true;
    }, err => {
      return false;
    });  new User(userData["user_Id"], userData["login"], userData["password"], userData["user_Type"], userData["fullName"], userData["date_Of_Birth"], userData["phone_Number"], userData["email"], userData["address"], userData["is_Valid"]));
    */
  }

  public get() {
    return this.http.get(this.accessPointUrl, { headers: this.headers, withCredentials: true });
  }

  public AddUser(user) {
    return this.http.post(this.accessPointUrl +'/AddUser', user, { headers: this.headers});
  }

  //-------CHECK DATA
  public IfLoginExists(login) {
    return this.http.get(this.accessPointUrl + '/IfLoginExists/' + login, { headers: this.headers });
  }

  public IfEmailExists(email) {
    return this.http.get(this.accessPointUrl + '/IfEmailExists/' + email, { headers: this.headers});
  }

  public SearchUser(user) {

    return this.http.post(this.accessPointUrl + "/SearchUser",user, { headers: this.headers });
  }

  
  //-------------------------user Account
  public GetUserById(id) {
    return this.http.get(this.accessPointUrl + "/GetUserById/"+id, { headers: this.headers });
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
