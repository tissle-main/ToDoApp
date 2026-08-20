import { Component, effect, inject, input, output, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryDto } from '../../../../api';

@Component({
  selector: 'app-category-item',
  imports: [ReactiveFormsModule],
  templateUrl: './category-item.component.html',
  styleUrl: './category-item.component.css',
})
export class CategoryItemComponent
{
  private readonly fb = inject(NonNullableFormBuilder);
  public readonly category = input.required<CategoryDto>();
  public readonly saveCategory = output<CategoryDto>();
  public readonly deleteCategory = output<string>();
  public readonly editing = signal(false);
  public readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(50)]],
  });

  constructor()
  {
    effect(() =>
    {
      this.form.patchValue({
        name: this.category().name ?? '',
      });
    });
  }

  public startEditing(): void
  {
    this.form.patchValue({
      name: this.category().name ?? '',
    });
    this.editing.set(true);
  }
  public cancel(): void
  {
    this.form.patchValue({
      name: this.category().name ?? '',
    });
    this.editing.set(false);
  }
  public save(): void
  {
    if (this.form.invalid)
    {
      this.form.markAllAsTouched();
      return;
    }
    this.saveCategory.emit({
      ...this.category(),
      name: this.form.getRawValue().name,
    });
    this.editing.set(false);
  }
  public remove(): void
  {
    if (this.category().id)
    {
      this.deleteCategory.emit(this.category().id!);
    }
  }
}
