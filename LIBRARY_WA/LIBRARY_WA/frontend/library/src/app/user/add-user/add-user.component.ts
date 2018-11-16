import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormsModule, FormBuilder, Validators } from '@angular/forms';
import { Http } from '@angular/http';
import { User } from '../../_models/User';
import { UserService } from '../../_services/user.service';

//import { createConnection } from 'net';
//import { createConnection } from 'mysql';
//import * as mysql from 'mysql';
@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styles: ['./add-user.component.css']

})
export class AddUserComponent implements OnInit {
  @Output() user = new EventEmitter<User>();
  addUserForm: FormGroup;
 // user: User;
  loading = false;
  submitted = false;
  returnUrl: string;

  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private userService: UserService) { }


  ngOnInit() {
    this.submitted = false;
    var names = ["Login", "E-mail", "Imię/nazwisko", "Data urodzenia", "Numer telefonu"]
    this.addUserForm = this.formBuilder.group({
      login: '',
      email: ['', Validators.pattern("/^[a-z0-9!#$%&'*+\/=?^_`{|}~.-]+@[a-z0-9]([a-z0-9-]*[a-z0-9])?(\.[a-z0-9]([a-z0-9-]*[a-z0-9])?)*$/i")],
      fullname: '',
      dateOfBirth: [''],
      phoneNumber: ['', Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}")],
      type: '',
      password: '',
      address: '',
    });
  }

  clearForm() {
    this.addUserForm.reset();
  }

  addUser() {
    if (this.addUserForm.invalid) {
      return;
    }
   
    var m = this.user;
    this.userService.addUser(m);//.subscribe(
    //  joggingRecord => this.joggingData.push(jogging))
    


 //   this.user = this.addUserForm.value();
   // this.userService.addUser(m);
  //    .subscribe(user =>
    //    this.user = user['records']);
    this.submitted = true;
  }
}
