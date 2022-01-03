//styles
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { Routes, RouterModule } from '@angular/router';
//services
import { UserService } from './_services/user.service';
import { BookService } from './_services/book.service';
import { DictionaryService } from './_services/dictionary.service';

import { HttpClientModule } from '@angular/common/http';

//components
import { AppComponent } from './app.component';
import { AddBookComponent } from './add-book/add-book.component';
import { AddUserComponent } from './user/add-user/add-user.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { UserAccountComponent } from './user-account/user-account.component';
import { SearchUserComponent } from './user/search-user/search-user.component';
import { SearchBookComponent } from './search-book/search-book.component';
import { UserComponent } from './user/user.component';
import { UserPIModule } from './user-account/details/user-pi/user-pi.module';
import { EditBookComponent } from './edit-book/edit-book.module';
import { HttpModule } from '@angular/http';

export const appRoutes: Routes = [

  {
    path: 'app-login',
    component: LoginComponent
  },
  {
    path: 'app-user',
    component: UserComponent,
    children: [
      {
        
        path: 'app-add-user',
        component: AddUserComponent
     
      },
      {
        path: 'app-search-user',
        component: SearchUserComponent
      },
    {
        path: '',
      component: SearchUserComponent
      }]
  },
  
  {
    path: 'app-add-book',
    component: AddBookComponent
  },
  {
    path: 'app-search-book',
    component: SearchBookComponent,
   
  },
  {
    path: 'app-edit-book',
    component: EditBookComponent
  },
  {
    path: 'app-home',
    component: HomeComponent
  },
  {
    path: 'app-user-account',
    component: UserAccountComponent,
    children: [
      {
        path: 'app-user-pi',
        component: UserPIModule
      }]
  },
  {
    path: '',
    component: HomeComponent
  },
];

@NgModule({
  declarations: [
    AppComponent,
    AddBookComponent,
    AddUserComponent,
    AddBookComponent,
    HomeComponent,
    LoginComponent,
    UserAccountComponent,
    SearchUserComponent,
    SearchBookComponent,
    UserComponent,
    UserPIModule,
    EditBookComponent
  ],
  imports: [
    RouterModule.forRoot(appRoutes),
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpModule,
    HttpClientModule
  ],
  providers: [UserService, BookService, DictionaryService],
  bootstrap: [AppComponent]
})
export class AppModule { }
