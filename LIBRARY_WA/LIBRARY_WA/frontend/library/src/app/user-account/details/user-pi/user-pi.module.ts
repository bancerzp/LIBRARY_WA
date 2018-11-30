import { NgModule, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { UserService } from '../../../_services/user.service';
import { map } from 'rxjs/operators';
import { User } from '../../../_models/User';
import { AppComponent } from '../../../app.component';

@Component({
  selector: 'app-user-pi',
  templateUrl: './user-pi.component.html',
  providers: [AppComponent]
})

@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ]
})
export class UserPIModule {
  user = new User(null, "", "", "", "", null, "", "", "", true);
  user_type: String;
  updateUserForm: FormGroup;
  submitted: boolean;
  reset: boolean;
  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private userService: UserService,
    private app: AppComponent,) { }

  createForm() {
    
  }

  ngOnInit() {
    this.reset = true;
    this.GetUserById();
    this.submitted = false;
    this.user_type = this.user.user_type == "l" ? "Bibliotekarz" : "Czytelnik";
    //var names = ["Login", "E-mail", "Imię/nazwisko", "Data urodzenia", "Numer telefonu"]
    this.updateUserForm = this.formBuilder.group({
      user_id: [this.user.user_id],
      login: [this.user.login],
      email: [this.user.email, [Validators.email, Validators.required], this.checkEmailExistsInDB.bind(this)],
      fullname: [this.user.fullname, [Validators.required]], //, Validators.pattern("\S")
      date_of_birth: [this.user.date_of_birth, Validators.required],
      phone_number: [this.user.phone_number, [Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}"), Validators.required]],
      user_type: this.user_type,
      password: this.user.password,
      password2: '',
      address: [this.user.address, Validators.required], //, Validators.pattern("/^\S*$/")
    });
  }

  UpdateUser() {
    if (this.app.IsExpired())
      return;
  //  this.userService.GetUserById(localStorage.getItem("user_id"));
  }

  GetUserById() {
    if (this.app.IsExpired())
      return;
    this.userService.GetUserById(localStorage.getItem("user_id")).subscribe((user: User) => this.user = user);
  }

  NewPassword() {
    if (this.app.IsExpired())
      return;
    this.reset = this.updateUserForm.get('password') == this.updateUserForm.get('password2');
  }

  checkEmailExistsInDB(control: FormControl ) {
    return this.userService.IfEmailExists(control.value).pipe(
      map(((res: any[]) => res.filter(user => user.email == control.value && user.user_id != localStorage.getItem("user_id")).length == 0 ? { 'emailTaken': false } : { 'emailTaken': true })))
  };
}
