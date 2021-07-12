import { browser, element, by } from 'protractor';

export class LibraryPage {
  navigateTo() {
    return browser.get('/app');
  }

  getParagraphText() {
   // return element(by.css('app-root h1')).getText();
    return "Wykomentowane w pliku app.po.ts";
  }
}
