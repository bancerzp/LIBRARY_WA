import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormGroup, FormsModule, FormBuilder, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { UserService } from '../_services/user.service';
//import { AlertService, AuthenticationService } from '../_services';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})

export class LoginComponent implements OnInit {
  loginForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  user: User;
  valid: boolean;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private userService: UserService) { }

  ngOnInit() {
    var valid = true;
    this.user = null;
    this.loginForm = this.formBuilder.group({
      login: ['', Validators.required],
      password: ['', Validators.required],
      check:['']
    });
    this.loginForm.setValue({check:'g'});
  }

  onClickForm() {
    this.loginForm.get('login').markAsPristine();
    this.loginForm.get('password').markAsPristine();
  }
 


  //f czyli fałszywy login/hasło
  login() {
    var loginData: User = { login: this.loginForm.get('login').value, password: this.loginForm.get('password').value  };
     this.userService.isLogged()
     .subscribe(user =>
      this.user = user['records']
      this.submitted = true;
    if this.user == null{
      valid = false;
      return;
    }
  }
}




