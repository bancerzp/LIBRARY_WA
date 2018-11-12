import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Routes, RouterModule } from '@angular/router';

import { HttpClientModule } from '@angular/common/http';

//components

import { AddUserComponent } from './add-user/add-user.component';
import { SearchUserComponent } from './search-user/search-user.component';


const appRoutes: Routes = [

  {
    path: 'app-add-user',
    component: AddUserComponent
  },
  {
    path: 'app-search-user',
    component: SearchUserComponent
  },
];

@NgModule({
  declarations: [
    AddUserComponent,
   
    SearchUserComponent,

  ],
  imports: [
  //RouterModule.forChild(appRoutes),
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    HttpClientModule
  ],
})
export class UserModule { }
