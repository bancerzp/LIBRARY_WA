import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormsModule, FormBuilder, Validators, AbstractControl, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { User } from '../../_models/User';
import { UserService } from '../../_services/user.service';
import { map } from 'rxjs/operators';
import { AppComponent } from '../../app.component';

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styles: ['./add-user.component.css'],
  providers: [AppComponent]

})
export class AddUserComponent implements OnInit {

  user: User = new User(null, "", "", "", "", new Date('1968-11-16T00:00:00'), "", "", "", true)
  addUserForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  ifEmailExists: any;
  ifLoginExists: any[];
  maxDate = new Date().toString();
  message: String;

  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private userService: UserService,
    private app: AppComponent,) { }

  createForm() {
    if (this.app.IsExpired("l"))
      return;
    this.user.password = Math.random().toString(36).substring(2, 6) + Math.random().toString(36).substring(2, 6);
   
    this.addUserForm = this.formBuilder.group({
      login: [''],
      email: ['', [Validators.email, Validators.required], this.CheckEmailExistsInDB.bind(this)],
      fullname: ['', [Validators.required]],
      dateOfBirth: ['', Validators.required],
      phoneNumber: ['',[Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}"), Validators.required]],
      userType: ['', Validators.required],
      password: [this.user.password, [Validators.minLength(5), Validators.required]],
      address: ['', Validators.required], 
      isValid: [true]
    });
   
  }

  ngOnInit() {
    this.submitted = false;
    this.createForm();
  }

  clearForm() {
    this.createForm();
  }
  
  AddUser() {
    if (this.app.IsExpired("l"))
      return;
    this.submitted = false;
    this.user.isValid = true;
    this.userService.AddUser(this.user).subscribe(
      (data: User) => {this.user = data; },
      //
      response => { this.message = (<any>response).error.alert });
    this.submitted = true;
  }


  CheckEmailExistsInDB(control: FormControl) {
    return this.userService.IfEmailExists(control.value).pipe(
      map(((res: bool) => res==true ? { 'emailTaken': false } : null)))
  };

  CheckLoginExistsInDB(control: FormControl) {
    return this.userService.IfLoginExists(control.value).pipe(
      map(((res: bool) => res==true ? { 'loginTaken': false } : null)))
  };
    }

