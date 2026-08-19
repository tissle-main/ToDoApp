import { inject, Service, signal } from "@angular/core";
import { Api, TaskDto } from "../../api";
import { finalize, switchMap } from "rxjs";

@Service()
export class TaskStore
{
  private readonly api = inject(Api);
  public readonly tasks = signal<TaskDto[]>([]);
  public readonly loading = signal(false);
  public readonly error = signal<string | null>(null);

  public load()
  {
    this.loading.set(true);
    this.api.getTasks([]).pipe(
      finalize(() => this.loading.set(false))
    ).subscribe({
      next: tasks => this.tasks.set(tasks),
      error: err => this.error.set(err)
    });
  }
  public create(dto: TaskDto)
  {
    this.api.createTask(dto).pipe(
      switchMap(id => this.api.getTasks([id]))
    ).subscribe({
      next: tasks =>
      {
        const newTask = tasks[0];
        this.tasks.update(oldTasks => [...oldTasks, newTask])
      },
      error: err => this.error.set(err)
    });
  }
  public update(dto: TaskDto)
  {
    this.api.updateTask(dto).pipe(
      switchMap(() => this.api.getTasks([dto.id!]))
    ).subscribe({
      next: tasks =>
      {
        const updatedTask = tasks[0];
        this.tasks.update(tasks =>
        {
          return tasks.map(task => task.id === updatedTask.id ? updatedTask : task)
        });
      },
      error: err => this.error.set(err)
    });
  }
  public delete(id: string)
  {
    this.api.deleteTasks([id]).subscribe({
      next: () =>
      {
        this.tasks.update(oldTasks =>
        {
          return [...oldTasks.filter(value => value.id !== id)];
        });
      },
      error: err => this.error.set(err)
    });
  }
}
