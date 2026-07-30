import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../features/auth/auth.service';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-login.page',
  imports: [ReactiveFormsModule],
  templateUrl: './login.page.html',
  styleUrl: './login.page.css',
})
export class LoginPage
{
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  public readonly loginForm = this.fb.group({
    email: [
      '',
      [Validators.required, Validators.email]
    ],
    password: [
      '',
      [
        Validators.required,
        Validators.pattern('^[A-Za-z0-9]{8,}$')
      ]
    ]
  });
  public readonly loading = signal(false);
  public readonly error = signal<any>(undefined);

  public onSubmit()
  {
    if (this.loginForm.invalid)
    {
      this.loginForm.markAllAsTouched();
      return;
    }
    const request = this.loginForm.getRawValue();
    this.loading.set(true);
    this.authService.loginUser(request).pipe(
      finalize(() => this.loading.set(false))
    ).subscribe({
      next: () => this.router.navigate(['/']),
      error: err => this.error.set(err)
    });
  }
}
