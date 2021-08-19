import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class RentService {
   
  private headers: HttpHeaders;
  private accessPointUrl: string = 'https://localhost:5001/api/renting';
 
  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token")  });
  }
  
  public RentBook(reservationId) {
    var values = [reservationId]
    return this.http.put(this.accessPointUrl + "/RentBook",values, { headers: this.headers });
  }

  public ReturnBook(rentId) {
    var values = [rentId];
    return this.http.post(this.accessPointUrl + "/ReturnBook",values, { headers: this.headers });
  }

  public GetRent(id) {
    return this.http.get(this.accessPointUrl + "/GetRent/" + id, { headers: new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token") }) });
  }
}
