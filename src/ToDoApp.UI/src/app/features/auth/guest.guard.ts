import { CanActivateFn, Router } from "@angular/router";
import { AuthTokenStore } from "./auth.token.store";
import { inject } from "@angular/core";

export const guestGuard: CanActivateFn = () =>
{
  const authTokenStore = inject(AuthTokenStore);
  const router = inject(Router);
  return authTokenStore.isAuthenticated() ? router.createUrlTree(['/']) : true;
};
