import { Component, input, OnInit, output, signal } from '@angular/core';
import { CategoryDto } from '../../../../../api';

@Component({
  selector: 'app-task-list-item-categories-form',
  imports: [],
  templateUrl: './task-list-item-categories-form.component.html',
  styleUrl: './task-list-item-categories-form.component.css',
})
export class TaskListItemCategoriesFormComponent implements OnInit
{
  public readonly categories = input.required<CategoryDto[]>();
  public readonly selectedCategories = input<string[]>([]);

  public readonly saveCategories = output<string[]>();
  public readonly close = output<void>();

  public readonly selectedIds = signal<string[]>([]);

  public toggle(categoryId: string, event: Event): void
  {
    const checked = (event.target as HTMLInputElement).checked;

    this.selectedIds.update(ids =>
    {
      if (checked)
      {
        return ids.includes(categoryId)
          ? ids
          : [...ids, categoryId];
      }

      return ids.filter(id => id !== categoryId);
    });
  }
  public save(): void
  {
    this.saveCategories.emit(this.selectedIds());
  }

  public cancel(): void
  {
    this.close.emit();
  }
  public ngOnInit(): void
  {
    this.selectedIds.set(this.selectedCategories());
  }
}
