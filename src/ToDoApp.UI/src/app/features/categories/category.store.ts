import { inject, Service, signal } from "@angular/core";
import { Api, CategoryDto } from "../../api";
import { finalize, switchMap } from "rxjs";

@Service()
export class CategoryStore
{
  private readonly api = inject(Api);

  public readonly categories = signal<CategoryDto[]>([]);
  public readonly loading = signal(false);
  public readonly error = signal<string | null>(null);

  public readonly selectedCategoryId = signal<string | null>(null);

  public selectCategory(id: string | null): void
  {
    this.selectedCategoryId.set(id);
  }

  public clearCategory(): void
  {
    this.selectedCategoryId.set(null);
  }

  public load()
  {
    this.loading.set(true);

    this.api.getCategories([]).pipe(
      finalize(() => this.loading.set(false))
    ).subscribe({
      next: categories => this.categories.set(categories),
      error: err => this.error.set(err)
    });
  }

  public create(dto: CategoryDto)
  {
    this.api.createCategory(dto).pipe(
      switchMap(id => this.api.getCategories([id]))
    ).subscribe({
      next: categories =>
      {
        const newCategory = categories[0];

        this.categories.update(oldCategories =>
          [...oldCategories, newCategory]
        );
      },
      error: err => this.error.set(err)
    });
  }

  public update(dto: CategoryDto)
  {
    this.api.updateCategory(dto).pipe(
      switchMap(() => this.api.getCategories([dto.id!]))
    ).subscribe({
      next: categories =>
      {
        const updatedCategory = categories[0];

        this.categories.update(categories =>
          categories.map(category =>
            category.id === updatedCategory.id
              ? updatedCategory
              : category
          )
        );
      },
      error: err => this.error.set(err)
    });
  }

  public delete(id: string)
  {
    this.api.deleteCategories([id]).subscribe({
      next: () =>
      {
        this.categories.update(oldCategories =>
          oldCategories.filter(value => value.id !== id)
        );

        if (this.selectedCategoryId() === id)
        {
          this.clearCategory();
        }
      },
      error: err => this.error.set(err)
    });
  }
}
