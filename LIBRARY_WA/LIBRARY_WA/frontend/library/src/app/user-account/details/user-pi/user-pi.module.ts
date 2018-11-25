import { NgModule, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { UserService } from '../../../_services/user.service';
import { map } from 'rxjs/operators';

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
  @Input() user;
  updateUserForm: FormGroup;
  submitted: boolean;
  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private userService: UserService) { }

  createForm() {
  
    this.updateUserForm = this.formBuilder.group({
      login: [],
      email: ['', [Validators.email, Validators.required], this.checkEmailExistsInDB.bind(this)],
      fullname: ['', [Validators.required]], //, Validators.pattern("\S")
      date_Of_Birth: ['', Validators.required],
      phone_Number: ['', [Validators.pattern("[0-9]{3}-[0-9]{3}-[0-9]{3}"), Validators.required]],
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



  checkEmailExistsInDB(control: FormControl ) {
    return this.userService.IfEmailExists(control.value).pipe(
      map(((res: any[]) => res.filter(user => user.email == control.value).length == 0 ? { 'emailTaken': false } : { 'emailTaken': true })))
  };
}
