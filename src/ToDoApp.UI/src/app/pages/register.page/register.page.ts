import { Component, inject, signal } from '@angular/core';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../features/auth/auth.service';
import { Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { fieldsMatchValidator } from '../../shared/field.match.validator';
import { ProblemDetails } from '../../api';

@Component({
  selector: 'app-register.page',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './register.page.html',
  styleUrl: './register.page.css',
})
export class RegisterPage
{
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  public readonly registerForm = this.fb.group(
    {
      email: [
        '',
        [Validators.required, Validators.email]
      ],
      password: [
        '',
        [
          Validators.required,
          Validators.pattern('^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[A-Za-z\\d]{8,}$')
        ]
      ],
      confirmPassword: [
        '',
        [Validators.required]
      ]
    },
    {
      validators: [fieldsMatchValidator("password", "confirmPassword")]
    }
  );
  public readonly loading = signal(false);
  public readonly error = signal<ProblemDetails | undefined>(undefined);

  public onSubmit()
  {
    if (this.registerForm.invalid)
    {
      this.registerForm.markAllAsTouched();
      return;
    }
    const request = this.registerForm.getRawValue();
    this.loading.set(true);
    this.authService.registerUser(request).pipe(
      finalize(() => this.loading.set(false))
    ).subscribe({
      next: () => this.router.navigate(['/login']),
      error: err => this.error.set(err.result as ProblemDetails)
    });
  }
}
