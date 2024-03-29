import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { JwtHelper } from 'angular2-jwt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {

  public login: MenuItem = { title: 'Logowanie', path: 'app-login', href: '#login' };
  public addUser: MenuItem = { title: 'Dodaj użytkownika', path: 'app-add-user', href: '#addUser' };
  public addBook: MenuItem = { title: 'Dodaj książkę', path: 'app-add-book', href: '#addBook' };
  public users: MenuItem = { title: 'Użytkownicy', path: 'app-user', href: '#users' };
  public user: MenuItem = { title: 'Moje konto', path: 'app-user-account', href: '#user' };
  public userData: MenuItem = { title: 'Konto użytkownika', path: 'app-user-account', href: '#user' };
  public searchBook: MenuItem = { title: 'Wyszukaj książkę', path: 'app-search-book', href: '#searchBook' };
  public searchUser: MenuItem = { title: 'Wyszukaj użytkownika', path: 'app-search-user', href: '#searchUser' };

  public menuReader: MenuItem[] = [this.searchBook, this.user];
  public menuGuest: MenuItem[] = [this.login, this.searchBook];
  public menuLibrarian: MenuItem[] = [this.users, this.searchBook, this.addBook, this.userData];
  public menu: MenuItem[]; //= this.menuLibrarian;

  public userFullname;
  public userType;
  public userId;
  public isLogged;

  constructor(private router: Router) { }

  ngOnInit() {
    this.isLogged = false;
    if (localStorage.getItem("userType") == "r") {
      this.menu = this.menuReader;
      this.isLogged = true;
    }
    else if (localStorage.getItem("userType") == "l") {
      this.menu = this.menuLibrarian;
      this.isLogged = true;
    }
    else {
      this.menu = this.menuGuest;
      this.isLogged = false;
    }
    this.userFullname = localStorage.getItem("user_fullname");
  }

  Logout() {
    localStorage.clear();
    this.isLogged = false;
    this.menu = this.menuGuest;
    this.router.navigateByUrl('/app-login');
  }

  GetUserId() {
    if (localStorage.getItem('token') == null)
      return;
    return localStorage.getItem("userId");
  }

  GetUserType() {
    if (localStorage.getItem('token') == null)
      return;
    return localStorage.getItem("userType");
  }

  IsExpired(userType) {
    //false bo nie wygasł token
    //  var token = localStorage.getItem('token');

    if (localStorage.getItem('token') == null) {
      this.Logout();
      window.location.reload();
      //  this.router.navigateByUrl('/app-login');
      return true;
    }
    let jwtHelper: JwtHelper = new JwtHelper();

    if (jwtHelper.isTokenExpired(localStorage.getItem('token'))) {
      this.Logout();
      window.location.reload();
      return true;
    } else
      if (userType == "l,r")
        return false;
    return (!this.GetUserType() == userType);
  }

  Login(userType) {
    if (userType == "l") {
      this.menu = this.menuLibrarian;
    } else if (userType == "r") {
      this.menu = this.menuReader;
    }
    this.isLogged = true;
    this.userFullname = localStorage.getItem("user_fullname");
    this.router.navigateByUrl('/');
  }

  SetVariable(userType, userId) {

    this.userId = userId;
    this.userType = userType;

  }

  RouteTo(path) {
    this.router.navigateByUrl(path)
  }
}

export class MenuItem {
  title: string;
  path: string;
  href: string;

}

//w zależności od otrzymanego typu osoby menu=.
//obserwatorzy do zmiany typu użytkownika.
//W komponencie login ustawienie na początku typu użytkownika jako g(gość)
