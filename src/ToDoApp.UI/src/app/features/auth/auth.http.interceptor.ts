import { inject } from '@angular/core';
import { HttpInterceptorFn } from '@angular/common/http';
import { AuthTokenStore } from './auth.token.store';

export const authInterceptor: HttpInterceptorFn = (request, next) =>
{
  const token = inject(AuthTokenStore).token();
  if(!token)
  {
    return next(request);
  }
  return next(
    request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    })
  );
};
