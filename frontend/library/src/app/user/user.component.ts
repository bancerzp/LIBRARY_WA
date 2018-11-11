import { Component, OnInit, NgModule } from '@angular/core';
import { appRoutes } from '../app.module'
import { RouterModule } from '@angular/router';
@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css']
  
})
  @NgModule({    imports: [RouterModule.forChild(appRoutes)]
  })
  export class UserComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
