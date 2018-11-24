import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { User } from '../../_models/User';
import { UserService } from '../../_services/user.service';
//import { AlertService, AuthenticationService } from '../_services';

@Component({
  selector: 'app-search-user',
  templateUrl: './search-user.component.html',
  styles: []
})
export class SearchUserComponent implements OnInit {
  @Output() user = new EventEmitter<User>();

  column=["Id. użytkownika","Imię i nazwisko","Data urodzenia","email","Numer telefonu","login","typ"]

  userData: User[];
  searchUserForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private userService: UserService) { }
  //  private router: Router,
  // private authenticationService: AuthenticationService,
  // private alertService: AlertService) { }*/

  ngOnInit() {
    this.submitted = false;
    this.searchUserForm = this.formBuilder.group({
      userId: '',
      userFullname: '',
      email: ['', Validators.pattern("/^[a-z0-9!#$%&'*+\/=?^_`{|}~.-]+@[a-z0-9]([a-z0-9-]*[a-z0-9])?(\.[a-z0-9]([a-z0-9-]*[a-z0-9])?)*$/i")],
      login: ''
    });
  }
  /*
  // reset login status
  this.authenticationService.logout();

  // get return url from route parameters or default to '/'
  this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';*/


  // convenience getter for easy access to form fields
  // get f() { return this.loginForm.controls; }

  searchUser() {
    this.submitted = true;
    var values;
    Object.keys(this.searchUserForm.controls).forEach((name) => {
      var s = this.searchUserForm.controls[name].value;
      if (s.replace(" ", "").replace("'", "").length == 0) {
        values.push('%');
      }
      else {
        values.push(s.replace("'", ""));
      }
    });

    return this.userService.SearchUser(values).subscribe((data: User[]) => this.userData = data)



    
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
