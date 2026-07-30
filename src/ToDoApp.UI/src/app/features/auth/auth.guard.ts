import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthTokenStore } from './auth.token.store';

export const authGuard: CanActivateFn = () =>
{
  const authTokenStore = inject(AuthTokenStore);
  const router = inject(Router);
  if(authTokenStore.isAuthenticated())
  {
    return true;
  }
  return router.createUrlTree(['/login']);
};
