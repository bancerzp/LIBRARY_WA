import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

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
      let userId = (<any>response).id;
      let fullname = (<any>response).fullname;
      let userType = (<any>response).userType
      localStorage.setItem("jwt", userId);
      localStorage.setItem("userId", userId);
      localStorage.setItem("fullname", fullname);
      c.setItem("userType", userType);
      return true;
    }, err => {
      return false;
    });  new User(userData["user_Id"], userData["login"], userData["password"], userData["user_Type"], userData["fullName"], userData["date_Of_Birth"], userData["phone_Number"], userData["email"], userData["address"], userData["is_Valid"]));
    */
  }

  public ChangeUserStatus(status) {
    return this.http.put(this.accessPointUrl + "/ChangeUserStatus", status, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });

  }

  public get() {
    return this.http.get(this.accessPointUrl, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public AddUser(user) {
    return this.http.post(this.accessPointUrl + '/AddUser', user,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }


  public RemoveUser(id) {
    return this.http.delete(this.accessPointUrl + "/RemoveUser/" + id,
      { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public SearchUser(user) {

    return this.http.post(this.accessPointUrl + "/SearchUser", user, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public UpdateUser(user) {
    return this.http.put(this.accessPointUrl + "/UpdateUser", user, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public ResetPassword(user) {
    return this.http.put(this.accessPointUrl + "/ResetPassword", user, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  //-------CHECK DATA
  public IfLoginExists(login) {
    return this.http.get(this.accessPointUrl + '/IfLoginExists/' + login, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public IfEmailExists(email) {
    return this.http.get(this.accessPointUrl + '/IfEmailExists/' + email, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  
  //-------------------------user Account
  public GetUserById(id) {
    return this.http.get(this.accessPointUrl + "/GetUserById/" + id, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public remove(payload) {
    return this.http.delete(this.accessPointUrl + '/' + payload.id, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }

  public update(payload) {
    return this.http.put(this.accessPointUrl + '/' + payload.id, payload, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }
}
