import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination-item',
  imports: [],
  templateUrl: './pagination-item.component.html',
  styleUrl: './pagination-item.component.css',
})
export class PaginationItemComponent
{
  public readonly pageNumber = input.required<number>();
  public readonly isActive = input.required<boolean>();
  public readonly click = output<number>();

  public onClick()
  {
    this.click.emit(this.pageNumber());
  }
}
