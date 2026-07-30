import { inject, Service, signal } from "@angular/core";
import { Api, CategoryDto } from "../../api";
import { finalize } from "rxjs";

@Service()
export class CategoryStore
{
  private readonly api = inject(Api);
  public readonly categories = signal<CategoryDto[]>([]);
  public readonly loading = signal(false);
  public readonly error = signal<string | null>(null);

  public load()
  {
    this.loading.set(true);
    this.api.getCategories().pipe(
      finalize(() => this.loading.set(false))
    ).subscribe({
      next: categories => this.categories.set(categories),
      error: err => this.error.set(err.message)
    });
  }
  public create(dto: CategoryDto)
  {
    this.api.createCategory(dto).subscribe();
  }
  public update(dto: CategoryDto)
  {
    this.api.updateCategory(dto).subscribe();
  }
  public delete(id: string)
  {
    this.api.deleteCategories([id]).subscribe();
  }
}
