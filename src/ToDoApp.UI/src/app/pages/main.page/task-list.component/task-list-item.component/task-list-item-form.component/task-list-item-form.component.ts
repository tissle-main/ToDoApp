import { Component, computed, inject, input, OnInit, output, signal } from '@angular/core';
import
  {
    NonNullableFormBuilder,
    ReactiveFormsModule,
    Validators
  } from '@angular/forms';
import { CategoryDto, TaskDto } from '../../../../../api';
import { TaskListItemCategoriesFormComponent } from '../task-list-item-categories-form.component/task-list-item-categories-form.component';

@Component({
  selector: 'app-task-list-item-form',
  imports: [
    ReactiveFormsModule,
    TaskListItemCategoriesFormComponent
  ],
  templateUrl: './task-list-item-form.component.html',
  styleUrl: './task-list-item-form.component.css',
})
export class TaskListItemFormComponent implements OnInit
{
  private readonly fb = inject(NonNullableFormBuilder);

  public readonly task = input<TaskDto | null>(null);
  public readonly categories = input<CategoryDto[]>([]);

  public readonly saveTask = output<TaskDto>();
  public readonly close = output<void>();

  public readonly editingCategories = signal(false);

  public readonly editedCategories = signal<string[]>([]);

  public readonly selectedCategories = computed(() =>
  {
    const selectedIds = this.editedCategories();

    return this.categories().filter(category =>
      category.id && selectedIds.includes(category.id)
    );
  });

  public readonly isEditing = computed(() =>
    this.task() !== null
  );

  public readonly form = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(100)]],
    description: ['', [Validators.maxLength(1000)]],
    done: false,
  });

  public ngOnInit(): void
  {
    const task = this.task();

    if (task)
    {
      this.form.patchValue({
        title: task.title,
        description: task.description ?? '',
        done: task.done ?? false,
      });

      this.editedCategories.set([
        ...(task.categories ?? [])
      ]);
    }
  }

  public openCategories(): void
  {
    this.editingCategories.set(true);
  }

  public closeCategories(): void
  {
    this.editingCategories.set(false);
  }

  public saveCategories(categoryIds: string[]): void
  {
    this.editedCategories.set([...categoryIds]);
    this.editingCategories.set(false);
  }

  public cancel(): void
  {
    this.close.emit();
  }

  public save(): void
  {
    if (this.form.invalid)
    {
      this.form.markAllAsTouched();
      return;
    }

    const task = this.task();

    this.saveTask.emit({
      ...(task ?? {}),
      ...this.form.getRawValue(),
      categories: [...this.editedCategories()],
    });
  }
}
