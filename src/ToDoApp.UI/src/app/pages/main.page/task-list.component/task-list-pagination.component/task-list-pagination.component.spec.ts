import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TaskListPaginationComponent } from './task-list-pagination.component';

describe('TaskListPaginationComponent', () => {
  let component: TaskListPaginationComponent;
  let fixture: ComponentFixture<TaskListPaginationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TaskListPaginationComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(TaskListPaginationComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
