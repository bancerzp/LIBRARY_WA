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
  loading = false;
  submitted = false;
  returnUrl: string;
  ifExists: boolean[];
  maxDate = new Date().toString();

  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private userService: UserService) { }


  ngOnInit() {
    this.submitted = false;
    var names = ["Login", "E-mail", "Imię/nazwisko", "Data urodzenia", "Numer telefonu"]
    this.addUserForm = this.formBuilder.group({
      login: ['', Validators.required],
      email: ['', [Validators.email, Validators.required]],
      fullname: ['', Validators.required],
      date_Of_Birth: ['', Validators.required],
      phone_Number: ['', [Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}"),Validators.required]],
      type: ['', Validators.required],
      password: '',
      address: ['', Validators.required],
      is_Valid: [true],
    });
  }

  clearForm() {
    this.addUserForm.reset();
  }

  addUser() {
  //  var result=this.userService.ifUserExists(this.user);
   
  //  if (this.addUserForm.invalid) {
   //   this.submitted = true;
   //   return;
   // }

   
    var m = this.user;
    this.userService.addUser(m).subscribe(
      data => { alert("Użytkownik dodany poprawnie") },
      Error => { alert("Błąd dodawania użytkownika") });
  
    


 //   this.user = this.addUserForm.value();
   // this.userService.addUser(m);
  //    .subscribe(user =>
    //    this.user = user['records']);
    this.submitted = true;
  }
}
