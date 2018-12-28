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
  pass: String;
  resetClicked: boolean;
  message: String;
  email: String;
  lib: boolean;
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
    this.user_type = this.app.GetUserType();
    this.resetClicked = false;
    this.reset = true;
    this.GetUserById();
    this.submitted = false;
    this.pass = this.user.password;
    this.email = this.user.email;

    if (this.user_type == 'l') {
      this.user.password = "";
    }
    //var names = ["Login", "E-mail", "Imię/nazwisko", "Data urodzenia", "Numer telefonu"]
    this.updateUserForm = this.formBuilder.group({
      user_id: [this.user.user_id],
      login: ['', [Validators.required, Validators.minLength(5)], this.CheckLoginExistsInDB.bind(this)],//Validators.pattern("[/S]*"),
      email: ['', [Validators.email, Validators.required], this.CheckEmailExistsInDB.bind(this)],
      fullname: [this.user.fullname, [Validators.required]],// //, Validators.pattern("\S")
      date_of_birth: [this.user.date_of_birth],//, Validators.required
      phone_number: [this.user.phone_number, [Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}"), Validators.required]],//
      user_type: this.user_type,
      password: [this.user.password],
      password2: '',
      address: [this.user.address, [Validators.required]], //, Validators.pattern("/^\S*$/")
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
    //,
    // response => { this.message = (<any>response).error.alert });
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
      localStorage.setItem("user_id", this.app.GetUserId());
    }
    //czy bibliotekarz przeglada swoje konto czy czytelnika
    
    this.userService.GetUserById(localStorage.getItem("user_id")).subscribe((user: User) => { this.user = user; this.pass = this.user.password },
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
      map(((res: boolean) => (res == true && control.value==this.email) ? { 'emailTaken': false } : null)))
  };
}
