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
  user:User;
  updateUserForm: FormGroup;
  submitted: boolean;
  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private userService: UserService) { }

  createForm() {
    this.updateUserForm = this.formBuilder.group({
      login: [this.user.login],
      email: [this.user.email, [Validators.email, Validators.required], this.checkEmailExistsInDB.bind(this)],
      fullname: [this.user.fullname, [Validators.required]], //, Validators.pattern("\S")
      date_of_birth: [this.user.date_of_birth, Validators.required],
      phone_number: [this.user.phone_number, [Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}"), Validators.required]],
      type: [this.user.user_type, Validators.required],
      password: this.user.password,
      address: [this.user.address, Validators.required], //, Validators.pattern("/^\S*$/")
      is_Valid: [true]
    });
  }

  ngOnInit() {
    this.GetUserById();
    this.submitted = false;
    //var names = ["Login", "E-mail", "Imię/nazwisko", "Data urodzenia", "Numer telefonu"]
    this.createForm();
  }

  UpdateUser() {
  //  this.userService.GetUserById(localStorage.getItem("user_id"));
  }

  GetUserById() {
    this.userService.GetUserById(localStorage.getItem("user_id")).subscribe((user: User) => this.user = user);
  }



  checkEmailExistsInDB(control: FormControl ) {
    return this.userService.IfEmailExists(control.value).pipe(
      map(((res: any[]) => res.filter(user => user.email == control.value).length == 0 ? { 'emailTaken': false } : { 'emailTaken': true })))
  };
}
