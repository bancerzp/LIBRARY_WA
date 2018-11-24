import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormsModule, FormBuilder, Validators, AbstractControl, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { User } from '../../_models/User';
import { UserService } from '../../_services/user.service';
import { map } from 'rxjs/operators';

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
  ifEmailExists: any;
  ifLoginExists: any;
  maxDate = new Date().toString();

  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private userService: UserService) { }

  createForm() {
    this.ifLoginExists = false;
    this.addUserForm = this.formBuilder.group({
      login: ['', [Validators.required, this.checkLoginExistsInDB.bind(this), Validators.minLength(5), Validators.maxLength(10)]],//Validators.pattern("[/S]*"),
      email: ['', [Validators.email, Validators.required], this.checkEmailExistsInDB.bind(this)],
      fullname: ['', [Validators.required]], //, Validators.pattern("\S")
      date_Of_Birth: ['', Validators.required],
      phone_Number: ['',[Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}"), Validators.required]],
      type: ['', Validators.required],
      password: '',
      address: ['', Validators.required], //, Validators.pattern("/^\S*$/")
      is_Valid: [true]
    });
  }

  ngOnInit() {
    this.submitted = false;
    //var names = ["Login", "E-mail", "Imię/nazwisko", "Data urodzenia", "Numer telefonu"]
    this.createForm();
  }

  clearForm() {
    console.log("wyczyszczone");
    this.createForm();
  }

  checkDate() {

   /*
      if (this.addUserForm.value("date_of_birth") > Date.now()) {
        this.addUserForm.get(""date_of_birth"").setValue([], Date.now());
        return false;
      }
    }
    else {

      return true;
    }
    */
  }


  addUser() {
  //  var result=this.userService.ifUserExists(this.user);
   
  //  if (this.addUserForm.invalid) {
   //   this.submitted = true;
   //   return;
   // }
    var m = this.user;
    this.userService.AddUser(m).subscribe(
      data => { alert("Użytkownik dodany poprawnie") },
      Error => { alert("Błąd dodawania użytkownika") });

 //   this.user = this.addUserForm.value();
   // this.userService.addUser(m);
  //    .subscribe(user =>
    //    this.user = user['records']);
    this.submitted = true;
  }


  checkEmailExistsInDB(control: FormControl) {
    return this.userService.IfEmailExists(control.value).pipe(
      map(((res: any[]) => res.filter(user => user.email == control.value).length == 0 ? { 'emailTaken': false } : { 'emailTaken': true })))
  };

  checkLoginExistsInDB(control: FormControl) {
    return this.userService.IfLoginExists(control.value).pipe(
      map(((res: any[]) => res.filter(user=>user.login==control.value).length == 0 ? { 'loginTaken': false } : { 'loginTaken': true })))
  };
    }

