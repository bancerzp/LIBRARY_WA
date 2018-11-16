import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormsModule, FormBuilder, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { UserService } from '../_services/user.service';
import { User } from '../_models/User';
//import { AlertService, AuthenticationService } from '../_services';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  @Output() logedUser = new EventEmitter<User>();


  loginForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  user: User;
  valid: boolean;
  loginData: User;
  test: string;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private userService: UserService,
    private router: Router) { }

  ngOnInit() {
    this.valid = true;
    this.user = null;
    this.loginForm = this.formBuilder.group({
      login: ['', Validators.required],
      password: ['', Validators.required],
      check: ['']
    });
  }

  onClickForm() {
    this.loginForm.get('login').markAsPristine();
    this.loginForm.get('password').markAsPristine();
  }


  //f czyli fałszywy login/hasło
  login() {
   // this.loginData = new User();//{ userId login: this.loginForm.get('login').value, password: this.loginForm.get('password').value };
   // this.loginData.login = this.loginForm.get('login').value;
   // this.loginData.password = this.loginForm.get('password').value;
    
    this.userService.isLogged(this.logedUser)
      .subscribe(userData => this.test = userData.toString());
   //     this.user = new User(userData["userId"], userData["login"], userData["password"], userData["userType"], userData["fullName"], userData["dateOfBirth"], userData["phoneNumber"], userData["email"], userData["address"]));
      this.submitted = true;
    if (this.user === null) {
      this.valid = false;
      this.router.navigate(['home']);
      return;
    }
    else {
      this.logedUser.emit(this.user);
    //  this.router.navigate(['home']);
    }
  }
}




