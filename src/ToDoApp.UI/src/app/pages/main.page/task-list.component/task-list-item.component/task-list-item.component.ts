import { Component, computed, input, output } from '@angular/core';
import { CategoryDto, TaskDto } from '../../../../api';

@Component({
  selector: 'app-task-list-item',
  imports: [],
  templateUrl: './task-list-item.component.html',
  styleUrl: './task-list-item.component.css',
})
export class TaskListItemComponent
{
  public readonly task = input.required<TaskDto>();
  public readonly categories = input<CategoryDto[]>([]);

  public readonly toggleDone = output<TaskDto>();
  public readonly deleteTask = output<string>();
  public readonly editTask = output<TaskDto>();

  public readonly taskCategories = computed(() =>
  {
    const categoryIds = this.task().categories ?? [];

    return this.categories().filter(category =>
      category.id && categoryIds.includes(category.id)
    );
  });

  public onDoneChange(event: Event): void
  {
    const checked = (event.target as HTMLInputElement).checked;

    this.toggleDone.emit({
      ...this.task(),
      done: checked,
    });
  }

  public remove(): void
  {
    if (this.task().id)
    {
      this.deleteTask.emit(this.task().id!);
    }
  }

  public startEdit(): void
  {
    this.editTask.emit(this.task());
  }
}
