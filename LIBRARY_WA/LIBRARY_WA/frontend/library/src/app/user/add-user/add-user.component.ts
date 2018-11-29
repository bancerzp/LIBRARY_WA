import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormsModule, FormBuilder, Validators, AbstractControl, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { User } from '../../_models/User';
import { UserService } from '../../_services/user.service';
import { map } from 'rxjs/operators';
import { AppComponent } from '../../app.component';

//import { createConnection } from 'net';
//import { createConnection } from 'mysql';
//import * as mysql from 'mysql';
@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styles: ['./add-user.component.css'],
  providers: [AppComponent]

})
export class AddUserComponent implements OnInit {
  @Output() user = new EventEmitter<User>();
  addUserForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  ifEmailExists: any;
  ifLoginExists: any[];
  maxDate = new Date().toString();

  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private userService: UserService,
    private app: AppComponent,) { }

  createForm() {
    
    this.addUserForm = this.formBuilder.group({
      login: ['', [Validators.required, Validators.minLength(5)], this.CheckLoginExistsInDB.bind(this)],//Validators.pattern("[/S]*"),
      email: ['', [Validators.email, Validators.required], this.CheckEmailExistsInDB.bind(this)],
      fullname: ['', [Validators.required]], //, Validators.pattern("\S")
      date_of_birth: ['', Validators.required],
      phone_number: ['',[Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}"), Validators.required]],
      type: ['', Validators.required],
      password: '',
      address: ['', Validators.required], //, Validators.pattern("/^\S*$/")
      is_valid: [true]
    });
  }

  ngOnInit() {
    this.submitted = false;
    //var names = ["Login", "E-mail", "Imię/nazwisko", "Data urodzenia", "Numer telefonu"]
    this.createForm();
  }

  clearForm() {
  
    this.createForm();
  }
  
  AddUser() {
    if (this.app.IsExpired())
      return;
  //  var result=this.userService.ifUserExists(this.user);
   
  //  if (this.addUserForm.invalid) {
   //   this.submitted = true;
   //   return;
   // }
    this.userService.AddUser(this.user).subscribe(
      data => { alert("Użytkownik dodany poprawnie"); this.ngOnInit() },
      Error => { alert("Błąd dodawania użytkownika") });

 //   this.user = this.addUserForm.value();
   // this.userService.addUser(m);
  //    .subscribe(user =>
    //    this.user = user['records']);
    this.submitted = true;
  }


  CheckEmailExistsInDB(control: FormControl) {
    return this.userService.IfEmailExists(control.value).pipe(
      map(((res: any[]) => res.filter(user => user.email == control.value).length > 0 ? { 'emailTaken': false } : null)))
  };

  CheckLoginExistsInDB(control: FormControl) {
    return this.userService.IfLoginExists(control.value).pipe(
      map(((res: any[]) => res.filter(user => user.login == control.value).length > 0 ? { 'loginTaken': false } : null)))
  };
    }

