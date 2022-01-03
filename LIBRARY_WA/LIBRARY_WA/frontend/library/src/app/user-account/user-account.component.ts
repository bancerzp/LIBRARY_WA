import { appRoutes } from '../app.module'
import { Component, OnInit, NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { User } from '../_models/User';

@Component({
  selector: 'app-user-account',
  templateUrl: './user-account.component.html',
  styleUrls: ['./user-account.component.css']
})

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)]
})
export class UserAccountComponent implements OnInit {
  userData: User[];
  constructor() { }

  ngOnInit() {
 
  }
 
}
