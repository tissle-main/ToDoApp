import { AfterViewInit, Component, computed, ElementRef, input, output, signal, ViewChild } from '@angular/core';
import { PaginationItemComponent } from './pagination-item.component/pagination-item.component';

@Component({
  selector: 'app-pagination-list',
  imports: [PaginationItemComponent],
  templateUrl: './pagination-list.component.html',
  styleUrl: './pagination-list.component.css',
})
export class PaginationListComponent implements AfterViewInit
{
  @ViewChild('container')
  private container!: ElementRef<HTMLDivElement>;

  protected readonly canScroll = signal(false);
  protected readonly canScrollLeft = signal(false);
  protected readonly canScrollRight = signal(false);
  protected readonly pages = computed(() =>
  {
    Array.from({ length: this.pageCount() }, (_, i) => i + 1)
  });
  public readonly pageCount = input.required<number>();
  public readonly activePage = input.required<number>();
  public readonly selectPage = output<number>();

  private updateButtons(): void
  {
    const el = this.container.nativeElement;
    this.canScroll.set(el.scrollWidth > el.clientWidth);
    this.canScrollLeft.set(el.scrollLeft > 0);
    this.canScrollRight.set(el.scrollLeft + el.clientWidth < el.scrollWidth - 1);
  }
  protected scrollLeft(): void
  {
    this.container.nativeElement.scrollBy({
      left: -250,
      behavior: 'smooth'
    });
  }
  protected scrollRight(): void
  {
    this.container.nativeElement.scrollBy({
      left: 250,
      behavior: 'smooth'
    });
  }
  public ngAfterViewInit(): void
  {
    queueMicrotask(() => this.updateButtons());
    const element = this.container.nativeElement;
    element.addEventListener('scroll', () => this.updateButtons());
    new ResizeObserver(() => this.updateButtons()).observe(element);
  }
  public onSelectPage(page: number)
  {
    this.selectPage.emit(page);
  }
}
