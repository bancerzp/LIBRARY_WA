import { NgModule, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { UserService } from '../../../_services/user.service';
import { map } from 'rxjs/operators';
import { User } from '../../../_models/User';

@Component({
  selector: 'app-user-pi',
  templateUrl: './user-pi.component.html'
})

@NgModule({
  declarations: [],
  imports: [
    CommonModule
  ]
})
export class UserPIModule {
  user = new User(null, "", "", "", "", null, "", "", "", true);
  updateUserForm: FormGroup;
  submitted: boolean;
  reset: boolean;
  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private userService: UserService) { }

  createForm() {
    
  }

  ngOnInit() {
    this.reset = true;
    this.GetUserById();
    this.submitted = false;
    //var names = ["Login", "E-mail", "Imię/nazwisko", "Data urodzenia", "Numer telefonu"]
    this.updateUserForm = this.formBuilder.group({
      login: [this.user.login],
      email: [this.user.email, [Validators.email, Validators.required], this.checkEmailExistsInDB.bind(this)],
      fullname: [this.user.fullname, [Validators.required]], //, Validators.pattern("\S")
      date_of_birth: [this.user.date_of_birth, Validators.required],
      phone_number: [this.user.phone_number, [Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}"), Validators.required]],
      type: [this.user.user_type, Validators.required],
      password: this.user.password,
      password2: '',
      address: [this.user.address, Validators.required], //, Validators.pattern("/^\S*$/")
    });
  }

  UpdateUser() {
  //  this.userService.GetUserById(localStorage.getItem("user_id"));
  }

  GetUserById() {
    this.userService.GetUserById(localStorage.getItem("user_id")).subscribe((user: User) => this.user = user);
  }

  NewPassword() {
    this.reset = this.updateUserForm.get('password') == this.updateUserForm.get('password2');
  }

  checkEmailExistsInDB(control: FormControl ) {
    return this.userService.IfEmailExists(control.value).pipe(
      map(((res: any[]) => res.filter(user => user.email == control.value && user.user_id != localStorage.getItem("user_id")).length == 0 ? { 'emailTaken': false } : { 'emailTaken': true })))
  };
}
