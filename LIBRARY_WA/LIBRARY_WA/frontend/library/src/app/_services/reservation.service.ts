import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class ReservationService {
   
  private headers: HttpHeaders;
  private accessPointUrl: string = 'https://localhost:5001/api/reservation';
 
  constructor(private http: HttpClient) {
    this.headers = new HttpHeaders({ 'Content-Type': 'application/json; charset=utf-8', 'Authorization': "Bearer " + localStorage.getItem("token")  });
  }
  
  public ReserveBook(bookId, userId) {
    var values = [bookId, userId]
    return this.http.put(this.accessPointUrl + "/ReserveBook", values, { headers: this.headers });
  }

  public CancelReservation(id) {
    return this.http.delete(this.accessPointUrl + "/CancelReservation/" + id, { headers: this.headers });
  }

  public GetReservation(id) {
    return this.http.get(this.accessPointUrl + "/GetReservation/" + id, { headers: this.headers });
  }
}
