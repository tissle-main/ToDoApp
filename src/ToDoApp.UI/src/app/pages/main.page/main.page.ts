import { Component, computed, inject } from '@angular/core';
import { AuthService } from '../../features/auth/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-main.page',
  imports: [],
  templateUrl: './main.page.html',
  styleUrl: './main.page.css',
})
export class MainPage
{
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  public readonly email = computed(() => this.authService.email());

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
