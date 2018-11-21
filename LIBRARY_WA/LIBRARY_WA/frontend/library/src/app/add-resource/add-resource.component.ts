import { Component, OnInit } from '@angular/core';
import { ResourceService } from '../_services/resource.service';
import { FormBuilder, Validators, FormGroup, FormControl } from '@angular/forms';
import { Http } from '@angular/http';
@Component({
  selector: 'app-add-resource',
  templateUrl: './add-resource.component.html',
  styleUrls: ['./add-resource.component.css'],
  providers: [ResourceService]
})
export class AddResourceComponent implements OnInit {
  addResourceForm: FormGroup;
  column: String[] = ["Id. książki", "ISBN", "Tytuł", "Autor", "Rok wydania", "Język wydania", "Rodzaj"];
  public author=["1","2"];
  public type = [];

  constructor(private formBuilder: FormBuilder,
    private http: Http,
    private resourceService: ResourceService,
  ) { }

  ngOnInit() {

    //pobierz wszystkie typy książki
    //pobierz wszystkie języki
   
    this.addResourceForm = this.formBuilder.group({
      book_id: 'idd',
      ISBN: ['', Validators.pattern("^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0 - 9X]{ 13}$ | 97[89][0 - 9]{ 10}$ | (?= (?: [0 - 9] + [- ]){ 4})[- 0 - 9]{ 17 } $)(?: 97[89][- ] ?) ? [0 - 9]{ 1, 5 } [- ] ? [0 - 9] + [- ] ? [0 - 9] + [- ] ? [0 - 9X]$")],
      title: 'full',
      authorFullName: [''],
      year: ['', Validators.pattern("[1-9][0-9]*")],
      language: '',
    });
  }
  addResource() {

  }


  getType() {
  //  return this.resourceService.GetBookType().pipe(
  };

  getAuthor() {
    //  return this.resourceService.GetAuthor().subscribe(authors => this.author = authors);
  }
  clearForm() {}
}
