import { Component, computed, inject } from '@angular/core';
import { AuthService } from '../../features/auth/auth.service';
import { Router } from '@angular/router';
import { CategoryListComponent } from './category-list.component/category-list.component';
import { TaskListComponent } from './task-list.component/task-list.component';

@Component({
  selector: 'app-main.page',
  imports: [CategoryListComponent, TaskListComponent],
  templateUrl: './main.page.html',
  styleUrl: './main.page.css',
})
export class MainPage
{
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  public readonly email = computed(() => this.authService.user()?.email);

  public logoutUser()
  {
    this.authService.logoutUser();
    this.router.navigate(["/login"]);
  }
  public deleteUser()
  {
    this.authService.deleteUser().subscribe(() =>
    {
      this.router.navigate(["/login"]);
    });
  }
}
