import { Component, OnInit, Output, EventEmitter, NgModule } from '@angular/core';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { FormGroup, FormsModule, FormBuilder, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { UserService } from '../_services/user.service';
import { User } from '../_models/User';
import { appRoutes } from '../app.module';
import { AppComponent } from "../app.component";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
@NgModule({
  imports: [RouterModule.forRoot(appRoutes)]
})

export class LoginComponent implements OnInit {
  @Output() logedUser = new EventEmitter<User>();


  loginForm: FormGroup;
  submitted = false;
  user: User;
  valid: Boolean;
  loginData: User;
  message: String;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private userService: UserService,
    private router: Router,
    private app:AppComponent) { }

  ngOnInit() {
    this.valid = true;
    this.user = null;
    this.loginForm = this.formBuilder.group({
      login: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onClickForm() {
    this.loginForm.get('login').markAsPristine();
    this.loginForm.get('password').markAsPristine();
  }

  login() {
      this.userService.IsLogged(this.logedUser).subscribe(response => {
      let token = (<any>response).token;
      let userId = (<any>response).id;
      let fullname = (<any>response).fullname;
      let userType = (<any>response).userType;
      let expires = (<any>response).expires;
      localStorage.setItem("token", token);
      localStorage.setItem("userId", userId);
      localStorage.setItem("user_fullname", fullname);
      localStorage.setItem("expires", expires);
      localStorage.setItem("userType", userType);
      this.app.SetVariable(userType,userId);
      this.valid = true;
      this.app.Login(localStorage.getItem("userType"));
        
      }, response => { this.message = (<any>response).error.alert;this.valid=false});
    this.submitted = true;
  }
  }
