import { Component, OnInit } from '@angular/core';
import { ResourceService } from '../_services/resource.service';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
import { map } from 'rxjs/operators';
import { Resource } from '../_models/resource';
import { Router } from '@angular/router';
@Component({
  selector: 'app-add-resource',
  templateUrl: './add-resource.component.html',
  styleUrls: ['./add-resource.component.css'],
  providers: [ResourceService]
})
export class AddResourceComponent implements OnInit {
  addResourceForm: FormGroup;
  column: String[] = ["Id. książki", "ISBN", "Tytuł", "Autor", "Rok wydania", "Język wydania", "Rodzaj"];
  public author=[];
  public type = [];
  public language = [];
  resource: Resource;

  constructor(private formBuilder: FormBuilder,
    private http: Http,
    private resourceService: ResourceService,
    private router: Router) { }

  ngOnInit() {

    //pobierz wszystkie typy książki
    this.getType();
    //pobierz wszystkie języki
    this.GetAuthor();
     //pobierz wszystkich autorów
    this.getLanguage();

    this.addResourceForm = this.formBuilder.group({
      book_id: ['', Validators.required],
      ISBN: ['', Validators.pattern("^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0 - 9X]{ 13}$ | 97[89][0 - 9]{ 10}$ | (?= (?: [0 - 9] + [- ]){ 4})[- 0 - 9]{ 17 } $)(?: 97[89][- ] ?) ? [0 - 9]{ 1, 5 } [- ] ? [0 - 9] + [- ] ? [0 - 9] + [- ] ? [0 - 9X]$")],
      title: '',
      authorFullName: [''],
      year: ['', Validators.pattern("[1-9][0-9]{3}")],
      language: '',
      type: [''],
    });
  }


  AddResource() {
    this.GetAuthor();
  }


  ClearForm() { }

  

  GetAuthor() {
    return this.resourceService.GetAuthor().subscribe((authors: any[]) => this.author = authors);
  }
  GetBookType() {
    return this.resourceService.GetBookType().subscribe((types: any[]) => this.type = types);
  };

  GetLanguage() {
    return this.resourceService.GetLanguage().pipe(
      map(((languages: any[]) => this.language = languages)));
  };
 
}
