import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskListItemCategoriesFormComponent } from './task-list-item-categories-form.component';

describe('TaskListItemCategoriesFormComponent', () => {
  let component: TaskListItemCategoriesFormComponent;
  let fixture: ComponentFixture<TaskListItemCategoriesFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskListItemCategoriesFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TaskListItemCategoriesFormComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
