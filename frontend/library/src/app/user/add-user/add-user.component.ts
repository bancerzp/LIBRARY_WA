import { Component, OnInit } from '@angular/core';
import { FormGroup, FormsModule, FormBuilder, Validators } from '@angular/forms';
import { Http } from '@angular/http';

//import { createConnection } from 'net';
//import { createConnection } from 'mysql';
//import * as mysql from 'mysql';
//#region
declare function addUserScript(): any;
//#endregion
@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styles: ['./add-user.component.css']

})
export class AddUserComponent implements OnInit {

  addUserForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;

  constructor(
    private formBuilder: FormBuilder,
    private http: Http) { }

  ngOnInit() {
    var names = ["Login", "E-mail", "Imię/nazwisko", "Data urodzenia", "Numer telefonu"]
    this.addUserForm = this.formBuilder.group({
      login: 'looogin',
      email: ['', Validators.pattern("/^[a-z0-9!#$%&'*+\/=?^_`{|}~.-]+@[a-z0-9]([a-z0-9-]*[a-z0-9])?(\.[a-z0-9]([a-z0-9-]*[a-z0-9])?)*$/i")],
      userFullname: 'full',
      birthDate: [''],
      phoneNumber: ['', Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}")],
      type: '',
      password: '',
    });
  }

  clearForm() {
    this.addUserForm.reset();
  }

  addUser() {
  }

}
