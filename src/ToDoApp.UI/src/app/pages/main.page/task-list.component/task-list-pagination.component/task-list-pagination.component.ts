import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-task-list-pagination',
  imports: [],
  templateUrl: './task-list-pagination.component.html',
  styleUrl: './task-list-pagination.component.css',
})
export class TaskListPaginationComponent
{
  public readonly page = input.required<number>();
  public readonly totalPages = input.required<number>();
  public readonly hasPreviousPage = input.required<boolean>();
  public readonly hasNextPage = input.required<boolean>();
  public readonly loading = input(false);

  public readonly previous = output<void>();
  public readonly next = output<void>();
}
