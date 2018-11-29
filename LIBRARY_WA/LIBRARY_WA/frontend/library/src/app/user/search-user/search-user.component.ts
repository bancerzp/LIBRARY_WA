import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { User } from '../../_models/User';
import { UserService } from '../../_services/user.service';
import { AppComponent } from '../../app.component';
//import { AlertService, AuthenticationService } from '../_services';

@Component({
  selector: 'app-search-user',
  templateUrl: './search-user.component.html',
  styles: [],
  providers: [AppComponent]
})
export class SearchUserComponent implements OnInit {
  @Output() user = new EventEmitter<User>();

  column=["Id. użytkownika","Imię i nazwisko","Data urodzenia","email","Numer telefonu","Login","Typ"]
  public values = [];
  userData: User[];
  searchUserForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  userType: string;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private userService: UserService,
    private app: AppComponent,) { }
  //  private router: Router,
  // private authenticationService: AuthenticationService,
  // private alertService: AlertService) { }*/

  ngOnInit() {
    this.submitted = false;
    this.userType = localStorage.getItem("user_type");
    this.submitted = false;
    
    this.searchUserForm = this.formBuilder.group({
      userId: '',
      user_fullname: '',
      email: ['', Validators.pattern("/^[a-z0-9!#$%&'*+\/=?^_`{|}~.-]+@[a-z0-9]([a-z0-9-]*[a-z0-9])?(\.[a-z0-9]([a-z0-9-]*[a-z0-9])?)*$/i")],
      login: '',
      phone_number: ''
    });
   // this.SearchUser();
  }
  /*
  // reset login status
  this.authenticationService.logout();

  // get return url from route parameters or default to '/'
  this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';*/


  // convenience getter for easy access to form fields
  // get f() { return this.loginForm.controls; }

  SearchUser() {
    if (this.app.IsExpired())
      return;
    this.submitted = false;
    this.values = [];

    Object.keys(this.searchUserForm.controls).forEach((name) => {
      var s = this.searchUserForm.controls[name].value;
      if (s.replace(" ", "").replace("'", "").length == 0) {
        this.values.push('%');
      }
      else {
        this.values.push(s.replace("'", ""));
      }
    });
    this.submitted = true;

    return this.userService.SearchUser(this.values).subscribe((data: User[]) => this.userData = data)
  }

  RemoveUser(id) {
    if (this.app.IsExpired())
      return;
    this.submitted = false;
    this.userService.RemoveUser(id).subscribe(data =>
    {
      this.userData = this.userData.filter(user => user.user_id != id);
    },
      Error => { alert(Error.message) });
    this.submitted = true;
  }

  UserAccount(id){
      if (this.app.IsExpired())
        return;
    this.app.RouteTo("app-user-account");
      localStorage.setItem("user_id",id)
    }

    
    // http.post('my/php/login.php?action=login', this.loginForm.login, this.loginForm.password).then(function (user) {
    //get data here
    // })
   

    /*  // stop here if form is invalid
      if (this.loginForm.invalid) {
        return;
      }
  
     this.loading = true;
      this.authenticationService.login(this.f.username.value, this.f.password.value)
        .pipe(first())
        .subscribe(
          data => {
            this.router.navigate([this.returnUrl]);
          },
          error => {
            this.alertService.error(error);
            this.loading = false;
      }
      );*/

}



/*interface User {
  userId: String;
  personId: String;
  login: String;
  password: String;
  pesel: String;
  userType: String;
  person: String;
}
*/
