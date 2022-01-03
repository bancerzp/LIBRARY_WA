import { CommonModule } from '@angular/common';
import { UserService } from '../../../_services/user.service';
import { map } from 'rxjs/operators';
import { User } from '../../../_models/User';
import { AppComponent } from '../../../app.component';
import { Http } from '@angular/http';
import { Component, NgModule } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';

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
  userType: String;
  updateUserForm: FormGroup;
  submitted: Boolean;
  reset: Boolean;
  pass: String;
  resetClicked: Boolean;
  message: String;
  email: String;
  changePass: Boolean;
  lib: Boolean;

  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private userService: UserService,
    private app: AppComponent,) { }

  createForm() {
    
  }

  ngOnInit() {
    if (this.app.IsExpired("l,r"))
      return;

    this.GetUserById();
    this.userType = this.app.GetUserType();
    this.resetClicked = false;
    this.reset = true;
    this.submitted = false;
    this.pass = this.user.password;
    this.email = this.user.email;

    if (this.userType == 'l') {
      this.user.password = "";
    }
    this.updateUserForm = this.formBuilder.group({
      userId: [this.user.userId],
      login: [this.user.login, [Validators.required, Validators.minLength(5)], this.CheckLoginExistsInDB.bind(this)],
      email: [this.user.email, [Validators.email, Validators.required], this.CheckEmailExistsInDB.bind(this)],
      fullname: [this.user.fullname, [Validators.required]],
      dateOfBirth: [this.user.dateOfBirth],
      phoneNumber: [this.user.phoneNumber, [Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}"), Validators.required]],
      userType: this.userType,
      password: [this.user.password],
      password2: '',
      address: [this.user.address, [Validators.required]], 
    });
  }

  CheckLoginExistsInDB(control: FormControl) {
    return this.userService.IfLoginExists(control.value).pipe(
      map(((res: Boolean) => res == true ? { 'loginTaken': false } : null)))
  };

  UpdateUser() {
    this.submitted = false;
    if (this.app.IsExpired("l,r"))
      return;
    if (this.reset == false) {
      this.submitted = false;
      this.message="Wpisane hasła się różnią! Nie można zapisać zmian!"
      this.submitted = true;
      return;
    }
    if (this.resetClicked == false) {
      this.user.password = this.pass;
    }
    this.userService.UpdateUser(this.user).subscribe(res => this.message="Dane zostały zaktualizowane",
      response => { this.message = (<any>response).error.alert });
    this.submitted = true;
  }

  GetUserById() {
    if (this.app.IsExpired("l,r"))
      return;
    if (this.app.GetUserType()=="r") {
      localStorage.setItem("userId", this.app.GetUserId());
    }
    //czy bibliotekarz przeglada swoje konto czy czytelnika
    
    this.userService.GetUserById(localStorage.getItem("userId")).subscribe((user: User) => { this.user = user; this.IfLoginChanged(); this.pass = this.user.password },
      response => { this.message = (<any>response).error.alert });
    this.lib = (localStorage.getItem("user_fullname") == this.user.fullname);
  }

  NewPassword() {
    this.resetClicked = false;
    if (this.app.IsExpired("l,r"))
      return;
    this.resetClicked = true;
    if (this.updateUserForm.get('password').value.length > 4 && this.updateUserForm.get('password').value.length < 21)
      this.reset = false;
    this.reset = (this.updateUserForm.get('password').value == this.updateUserForm.get('password2').value ); 
  }

  NewPasswordLib() {
    this.user.password = Math.random().toString(36).substring(2, 6) + Math.random().toString(36).substring(2, 6);
    this.updateUserForm.get("password").setValue(this.user.password);
    this.userService.ResetPassword(this.user).subscribe(resp=>alert("Hasło zostało zresetowane"),er=> alert("Błąd"));
  }


  CheckEmailExistsInDB(control: FormControl) {
    return this.userService.IfEmailExists(control.value).pipe(
      map(((res: Boolean) => (res == false && control.value==this.email) ? { 'emailTaken': false } : null)))
  };

  IfLoginChanged() {
    this.changePass=("000000000000" + this.user.userId).endsWith(this.user.login.toString());
  }
}
