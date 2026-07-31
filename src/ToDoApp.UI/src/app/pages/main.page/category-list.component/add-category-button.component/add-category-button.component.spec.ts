import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddCategoryButtonComponent } from './add-category-button.component';

describe('AddCategoryButtonComponent', () => {
  let component: AddCategoryButtonComponent;
  let fixture: ComponentFixture<AddCategoryButtonComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddCategoryButtonComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(AddCategoryButtonComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
