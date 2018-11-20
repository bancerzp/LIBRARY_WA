import { Component, OnInit, NgModule } from '@angular/core';
import { AppModule, appRoutes } from '../app.module'
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-user-account',
  templateUrl: './user-account.component.html',
  styleUrls: ['./user-account.component.css']
})

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)]
})
export class UserAccountComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
