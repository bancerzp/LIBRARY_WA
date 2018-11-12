import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GridJoggingComponentComponent } from './grid-jogging-component.component';

describe('GridJoggingComponentComponent', () => {
  let component: GridJoggingComponentComponent;
  let fixture: ComponentFixture<GridJoggingComponentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GridJoggingComponentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GridJoggingComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
