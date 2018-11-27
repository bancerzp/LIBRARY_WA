//styles
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Routes, RouterModule } from '@angular/router';
//services
import { WorkoutService } from './_services/workout.service';
import { UserService } from './_services/user.service';
import { BookService } from './_services/book.service';

import { HttpClientModule } from '@angular/common/http';
import { DecimalPipe } from '@angular/common';
import { DatePipe } from '@angular/common';

//components
import { AppComponent } from './app.component';
import { AddBookComponent } from './add-book/add-book.component';
import { AddUserComponent } from './user/add-user/add-user.component';
import { HomeComponent } from './home/home.component';
import { LoginComponent } from './login/login.component';
import { UserAccountComponent } from './user-account/user-account.component';
import { SearchUserComponent } from './user/search-user/search-user.component';
import { SearchBookComponent } from './search-book/search-book.component';
import { GridJoggingComponentComponent } from './grid-jogging-component/grid-jogging-component.component';
import { UserComponent } from './user/user.component';
import { BookReservationModule } from './user-account/details/book-reservation/book-reservation.module';
import { BookRenthModule } from './user-account/details/book-renth/book-renth.module';
import { CurrentRentModule } from './user-account/details/current-rent/current-rent.module';
import { UserPIModule } from './user-account/details/user-pi/user-pi.module';
import { EditBookComponent } from './search-book/edit-book/edit-book.module';

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
    children:[{
      path: 'app-edit-book',
      component: EditBookComponent
    },]
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

        path: 'app-book-reservation',
        component: BookReservationModule

      },
      {
        path: 'app-book-renth',
        component: BookRenthModule
      },
    {

      path: 'app-current-rent',
      component: CurrentRentModule

      },
      {
        path: 'app-user-pi',
        component: UserPIModule
      },
    {
        path: '',
      component: CurrentRentModule
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
    CurrentRentModule,
    GridJoggingComponentComponent,
    UserComponent,
    UserPIModule,
    BookReservationModule,
    BookRenthModule,
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
  providers: [WorkoutService,UserService, BookService],
  bootstrap: [AppComponent]
})
export class AppModule { }
