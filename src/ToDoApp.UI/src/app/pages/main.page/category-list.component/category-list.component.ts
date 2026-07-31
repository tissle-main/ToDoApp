import { Component, computed, inject, OnInit } from '@angular/core';
import { AddCategoryButtonComponent } from './add-category-button.component/add-category-button.component';
import { CategoryItemComponent } from './category-item.component/category-item.component';
import { CategoryStore } from '../../../features/categories/category.store';
import { CategoryDto } from '../../../api';

@Component({
  selector: 'app-category-list',
  imports: [CategoryItemComponent, AddCategoryButtonComponent],
  templateUrl: './category-list.component.html',
  styleUrl: './category-list.component.css',
})
export class CategoryListComponent implements OnInit
{
  private readonly categoryStore = inject(CategoryStore);
  public readonly loading = computed(() => this.categoryStore.loading());
  public readonly categories = computed(() => this.categoryStore.categories());

  public addCategory(dto: CategoryDto)
  {
    this.categoryStore.create(dto);
  }
  public saveCategory(dto: CategoryDto)
  {
    this.categoryStore.update(dto);
  }
  public deleteCategory(id: string)
  {
    this.categoryStore.delete(id);
  }
  public ngOnInit()
  {
    this.categoryStore.load();
  }
}
