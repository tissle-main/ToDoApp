import { computed, inject, Service, signal } from "@angular/core";
import { Api, TaskDto } from "../../api";
import
  {
    debounceTime,
    distinctUntilChanged,
    finalize,
    switchMap,
    Subject
  } from "rxjs";

@Service()
export class TaskStore
{
  private readonly api = inject(Api);
  private readonly searchSubject = new Subject<string>();
  public readonly tasks = signal<TaskDto[]>([]);
  public readonly loading = signal(false);
  public readonly error = signal<string | null>(null);
  public readonly search = signal<string | undefined>(undefined);
  public readonly category = signal<string | undefined>(undefined);
  public readonly done = signal<boolean | undefined>(undefined);
  public readonly page = signal(0);
  public readonly pageSize = signal(10);
  public readonly totalCount = signal(0);
  public readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );
  public readonly hasPreviousPage = computed(() =>
    this.page() > 0
  );
  public readonly hasNextPage = computed(() =>
    (this.page() + 1) * this.pageSize() < this.totalCount()
  );
  public readonly noCategories = signal<boolean>(false);

  constructor()
  {
    this.searchSubject.pipe(
      debounceTime(400),
      distinctUntilChanged()
    ).subscribe(search =>
    {
      this.search.set(search || undefined);
      this.page.set(0);
      this.load();
    });
  }

  public load(): void
  {
    const skip = this.page() * this.pageSize();
    const take = this.pageSize();

    this.loading.set(true);
    this.error.set(null);

    this.api.getTasksByFilter(
      this.search(),
      this.category(),
      this.done(),
      skip,
      take
    ).pipe(
      finalize(() => this.loading.set(false))
    ).subscribe({
      next: response =>
      {
        if (this.noCategories() === true)
        {
          response.tasks = response.tasks.filter(task => (task.categories ?? []).length === 0);
        }
        this.tasks.set(response.tasks);
        this.totalCount.set(response.totalCount);
      },
      error: err => this.error.set(err)
    });
  }
  public setSearch(search: string): void
  {
    this.searchSubject.next(search);
  }
  public setCategory(category?: string): void
  {
    if (category === "reserved-option-none")
    {
      this.noCategories.set(true);
      this.category.set(undefined);
    }
    else
    {
      this.noCategories.set(false);
      this.category.set(category || undefined);
    }
    this.page.set(0);
    this.load();
  }
  public setDone(done?: boolean): void
  {
    this.done.set(done);
    this.page.set(0);
    this.load();
  }
  public clearFilters(): void
  {
    this.search.set(undefined);
    this.category.set(undefined);
    this.done.set(undefined);
    this.page.set(0);
    this.load();
  }
  public nextPage(): void
  {
    if (this.loading() || !this.hasNextPage())
    {
      return;
    }

    this.page.update(page => page + 1);
    this.load();
  }
  public previousPage(): void
  {
    if (this.loading() || !this.hasPreviousPage())
    {
      return;
    }

    this.page.update(page => page - 1);
    this.load();
  }
  public create(dto: TaskDto): void
  {
    this.api.createTask(dto).pipe(
      switchMap(id => this.api.getTasks([id]))
    ).subscribe({
      next: () =>
      {
        // Перезавантажуємо поточну сторінку,
        // бо нова задача може не відповідати активному фільтру.
        this.load();
      },
      error: err => this.error.set(err)
    });
  }
  public update(dto: TaskDto): void
  {
    this.api.updateTask(dto).subscribe({
      next: () => this.load(),
      error: err => this.error.set(err)
    });
  }
  public delete(id: string): void
  {
    this.api.deleteTasks([id]).subscribe({
      next: () =>
      {
        // Якщо видалили останній елемент сторінки,
        // можна повернутися на попередню.
        if (this.tasks().length === 1 && this.page() > 0)
        {
          this.page.update(page => page - 1);
        }

        this.load();
      },
      error: err => this.error.set(err)
    });
  }
}
