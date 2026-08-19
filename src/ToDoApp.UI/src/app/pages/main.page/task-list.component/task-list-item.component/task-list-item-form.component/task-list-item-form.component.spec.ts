import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskListItemFormComponent } from './task-list-item-form.component';

describe('TaskListItemFormComponent', () => {
  let component: TaskListItemFormComponent;
  let fixture: ComponentFixture<TaskListItemFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskListItemFormComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TaskListItemFormComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
