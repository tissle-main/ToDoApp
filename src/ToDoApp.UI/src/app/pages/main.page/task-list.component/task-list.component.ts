import { Component, OnInit, inject, computed, signal } from '@angular/core';
import { TaskStore } from '../../../features/tasks/task.store';
import { CategoryStore } from '../../../features/categories/category.store';
import { TaskDto } from '../../../api';
import { TaskListItemComponent } from './task-list-item.component/task-list-item.component';
import { TaskListItemFormComponent } from './task-list-item.component/task-list-item-form.component/task-list-item-form.component';

@Component({
  selector: 'app-task-list',
  imports: [TaskListItemComponent, TaskListItemFormComponent],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css',
})
export class TaskListComponent implements OnInit
{
  private readonly taskStore = inject(TaskStore);
  private readonly categoryStore = inject(CategoryStore);
  public readonly loading = computed(() => this.taskStore.loading());
  public readonly tasks = computed(() =>
  {
    const tasks = this.taskStore.tasks();
    const categoryId = this.categoryStore.selectedCategoryId();

    if (!categoryId)
    {
      return tasks;
    }

    return tasks.filter(task =>
      task.categories?.includes(categoryId)
    );
  });
  public readonly categories = computed(() => this.categoryStore.categories());
  public readonly showTaskForm = signal(false);
  public readonly editingTask = signal<TaskDto | null>(null);
  public readonly selectedCategoryId = computed(() =>
    this.categoryStore.selectedCategoryId()
  );
  
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
  public ngOnInit()
  {
    this.taskStore.load();
  }
}
