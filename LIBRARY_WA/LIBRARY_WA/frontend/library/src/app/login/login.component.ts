import { Component, OnInit } from '@angular/core';
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
  loginForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  user: User;
  valid: boolean;
  loginData: User;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private userService: UserService) { }

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

    this.userService.isLogged(this.loginData)
     .subscribe(user =>
     this.user = user['records']);

      this.submitted = true;
    if (this.user == null){
      this.valid = false;
      return;
    }
  }
}




