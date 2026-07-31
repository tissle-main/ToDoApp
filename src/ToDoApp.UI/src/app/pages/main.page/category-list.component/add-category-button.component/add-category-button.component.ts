import { Component, inject, output, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategoryDto } from '../../../../api';

@Component({
  selector: 'app-add-category-button',
  imports: [ReactiveFormsModule],
  templateUrl: './add-category-button.component.html',
  styleUrl: './add-category-button.component.css',
})
export class AddCategoryButtonComponent
{
  private readonly fb = inject(NonNullableFormBuilder);
  public readonly addCategory = output<CategoryDto>();

  public readonly adding = signal(false);
  public readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(50)]]
  });
  public startAdding(): void
  {
    this.adding.set(true);
  }
  public cancel(): void
  {
    this.form.reset();
    this.adding.set(false);
  }
  public save(): void
  {
    if (this.form.invalid)
    {
      this.form.markAllAsTouched();
      return;
    }
    const { name } = this.form.getRawValue();
    this.addCategory.emit({ name: name });
    this.form.reset();
    this.adding.set(false);
  }
}
