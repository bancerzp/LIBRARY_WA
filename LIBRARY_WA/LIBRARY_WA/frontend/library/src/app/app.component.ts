import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent  {
 
  public login: MenuItem = { title: 'Logowanie', path: 'app-login', href: '#login' };
  public addUser: MenuItem = { title: 'Dodaj użytkownika', path: 'app-add-user', href: '#addUser' };
  public addBook: MenuItem = { title: 'Dodaj książkę', path: 'app-add-book', href: '#addBook' };
  public users: MenuItem = { title: 'Użytkownicy', path: 'app-user', href: '#users' };
  public user: MenuItem = { title: 'Moje konto', path: 'app-user-account', href: '#user' };
  public searchBook: MenuItem = { title: 'Wyszukaj książkę', path: 'app-search-book', href: '#searchBook' };
  public searchUser: MenuItem = { title: 'Wyszukaj użytkownika', path: 'app-search-user', href: '#searchUser' };
  public menuUser: MenuItem[]=[this.searchBook,this.user];
  public menuGuest: MenuItem[] = [this.login, this.searchBook];
  public menuLibrarian: MenuItem[] = [this.users, this.searchBook, this.addBook, this.login,this.user];
  public menu: MenuItem[] = this.menuLibrarian;
 
}

export class MenuItem {
  title: string;
  path: string;
  href: string;
}

//w zależności od otrzymanego typu osoby menu=.
//obserwatorzy do zmiany typu użytkownika.
//W komponencie login ustawienie na początku typu użytkownika jako g(gość)
