import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PaginationItemComponent } from './pagination-item.component';

describe('PaginationItemComponent', () => {
  let component: PaginationItemComponent;
  let fixture: ComponentFixture<PaginationItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PaginationItemComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(PaginationItemComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
