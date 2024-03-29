import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { User } from '../../_models/User';
import { UserService } from '../../_services/user.service';
import { AppComponent } from '../../app.component';
import { Status } from '../../_models/Status';
//import { AlertService, AuthenticationService } from '../_services';

@Component({
  selector: 'app-search-user',
  templateUrl: './search-user.component.html',
  styles: [],
  providers: [AppComponent]
})
export class SearchUserComponent implements OnInit {
  @Output() user = new EventEmitter<User>();

  column = ["Id. użytkownika", "Imię i nazwisko", "Data urodzenia", "email", "Numer telefonu", "Login", "Typ"]
  public values = [];
  userData: User[];
  searchUserForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  userType: string;
  message: String;

  constructor(
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private userService: UserService,
    private app: AppComponent, ) { }

  ngOnInit() {
    if (this.app.IsExpired("l"))
      return;
    this.submitted = false;
    this.userType = localStorage.getItem("userType");
    this.submitted = false;

    this.searchUserForm = this.formBuilder.group({
      userId: '',
      user_fullname: '',
      email: ['', Validators.pattern("/^[a-z0-9!#$%&'*+\/=?^_`{|}~.-]+@[a-z0-9]([a-z0-9-]*[a-z0-9])?(\.[a-z0-9]([a-z0-9-]*[a-z0-9])?)*$/i")],
      login: '',
      phoneNumber: ''
    });
  }

  SearchUser() {
    if (this.app.IsExpired("l"))
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

    return this.userService.SearchUser(this.values).subscribe((data: User[]) => this.userData = data,
      response => { this.message = (<any>response).error.alert });
  }

  RemoveUser(id) {
    if (this.app.IsExpired("l"))
      return;
    this.submitted = false;
    this.userService.RemoveUser(id).subscribe(data => {
      this.userData = this.userData.filter(user => user.userId != id);
      this.message = "Użytkownik o id: " + id + " został usunięty";
    },
      response => { this.message = (<any>response).error.alert });
    this.submitted = true;
  }

  UserAccount(id) {
    if (this.app.IsExpired("l"))
      return;
    this.app.RouteTo("app-user-account");
    localStorage.setItem("userId", id)
  }

  ChangeUserStatus(id,status) {
    this.userService.ChangeUserStatus(new Status(id, status)).subscribe(data => {
      if (status) {
        this.message = "Użytkownik został odblokowany";
      }
      else {
        this.message = "Użytkownik został zablokowany";
      }
      this.SearchUser();
    },
      response => { this.message = (<any>response).error.alert });

  }
  

}
