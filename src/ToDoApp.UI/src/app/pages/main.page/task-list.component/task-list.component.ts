import { Component, OnInit, inject, computed, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TaskStore } from '../../../features/tasks/task.store';
import { CategoryStore } from '../../../features/categories/category.store';
import { TaskDto } from '../../../api';
import { TaskListItemComponent } from './task-list-item.component/task-list-item.component';
import { TaskListItemFormComponent } from './task-list-item.component/task-list-item-form.component/task-list-item-form.component';

@Component({
  selector: 'app-task-list',
  imports: [
    FormsModule,
    TaskListItemComponent,
    TaskListItemFormComponent
  ],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
})
export class TaskListComponent implements OnInit
{
  public readonly taskStore = inject(TaskStore);
  public readonly categoryStore = inject(CategoryStore);

  public readonly loading = computed(() =>
    this.taskStore.loading()
  );

  public readonly tasks = computed(() =>
    this.taskStore.tasks()
  );

  public readonly categories = computed(() =>
    this.categoryStore.categories()
  );

  public readonly showTaskForm = signal(false);
  public readonly editingTask = signal<TaskDto | null>(null);

  // UI state

  public readonly search = signal('');
  public readonly selectedDone = signal<boolean | undefined>(undefined);

  public addTask(): void
  {
    this.editingTask.set(null);
    this.showTaskForm.set(true);
  }

  public editTask(task: TaskDto): void
  {
    this.editingTask.set(task);
    this.showTaskForm.set(true);
  }

  public saveTask(dto: TaskDto): void
  {
    if (dto.id)
    {
      this.taskStore.update(dto);
    }
    else
    {
      this.taskStore.create(dto);
    }

    this.showTaskForm.set(false);
  }

  public deleteTask(id: string): void
  {
    this.taskStore.delete(id);
  }

  public closeTaskForm(): void
  {
    this.showTaskForm.set(false);
  }

  public onSearchChange(value: string): void
  {
    this.search.set(value);

    this.taskStore.setSearch(value);
  }

  public setDone(value: boolean | undefined): void
  {
    this.selectedDone.set(value);

    this.taskStore.setDone(value);
  }

  public selectCategory(category: string | undefined): void
  {
    this.taskStore.setCategory(category);
  }

  public ngOnInit(): void
  {
    this.taskStore.load();
  }
}
