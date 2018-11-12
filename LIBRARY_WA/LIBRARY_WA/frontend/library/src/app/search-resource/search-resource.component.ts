import { Component, Input, Output, OnInit, EventEmitter } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { Http } from '@angular/http';
import { ResourceService } from '../_services/resource.service';
import { Resource } from '../_models/resource';
@Component({
  selector: 'app-search-resource',
  templateUrl: './search-resource.component.html',
  styleUrls: ['./search-resource.component.css'],
  providers: [ResourceService]
})
export class SearchResourceComponent implements OnInit {
  @Output() recordDeleted = new EventEmitter<any>();
  @Output() newClicked = new EventEmitter<any>();
  @Output() editClicked = new EventEmitter<any>();
  //@Input()
  resultData: Resource[];


  searchResourceForm: FormGroup;
  column: String[] = ["Id. książki", "ISBN", "Tytuł", "Autor", "Rok wydania", "Język wydania", "Rodzaj"];
  columnAddReader:String[] =["Zarezerwuj"]
  columnAddLibrarian: String[] = ["Zarezerwuj","Edytuj","Usuń"]


  constructor(
    private formBuilder: FormBuilder,
    private http: Http,
    private resourceService: ResourceService,
  ) {
   // resourceService.get().subscribe((data: any) => this.resultData = data);
  }
 
  public deleteRecord(record) {
    this.recordDeleted.emit(record);
  }

  public editRecord(record) {
    const clonedRecord = Object.assign({}, record);
    this.editClicked.emit(clonedRecord);
  }

  public newRecord() {
    this.newClicked.emit();
  }

  areResources() {
    return this.resultData.length != 0;
  }

  //ngOnInit() {
  //  this.ResourceService.readProducts()
  //    .subscribe(products =>
  //      this.resourceData = products['records']
  //    );
 // }


  ngOnInit() {

    //pobierz wszystkie typy książki
    //pobierz wszystkie języki
    var names = this.column;
    this.searchResourceForm = this.formBuilder.group({
    id: 'idd',
    ISBN: ['', Validators.pattern("^(?:ISBN(?:-1[03])?:? )?(?=[0-9X]{10}$|(?=(?:[0-9]+[- ]){3})[- 0 - 9X]{ 13}$ | 97[89][0 - 9]{ 10}$ | (?= (?: [0 - 9] + [- ]){ 4})[- 0 - 9]{ 17 } $)(?: 97[89][- ] ?) ? [0 - 9]{ 1, 5 } [- ] ? [0 - 9] + [- ] ? [0 - 9] + [- ] ? [0 - 9X]$")],
    title: 'full',
    authorFullName: [''],
    year: ['', Validators.pattern("[1-9][0-9]*")],
    language: '',
  });
  }

  clearForm() {
    this.searchResourceForm.reset();
  }
}
//tu tez przekazac zmienna czy jest ktos zalogowany
