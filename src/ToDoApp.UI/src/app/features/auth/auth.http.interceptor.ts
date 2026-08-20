import { inject } from '@angular/core';
import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { AuthTokenStore } from './auth.token.store';
import { AuthService } from './auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (request, next) =>
{
  const tokenStore = inject(AuthTokenStore);
  const authService = inject(AuthService);

  if (request.url.endsWith('/auth/refresh-token'))
  {
    return next(
      request.clone({
        withCredentials: true
      })
    );
  }

  const token = tokenStore.token();

  const authRequest = request.clone({
    withCredentials: true,
    ...(token && {
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    })
  });

  return next(authRequest).pipe(
    catchError((error: HttpErrorResponse) =>
    {
      if (error.status !== 401)
      {
        return throwError(() => error);
      }

      return authService.refreshAccessToken().pipe(
        switchMap(response =>
        {
          return next(
            request.clone({
              withCredentials: true,
              setHeaders: {
                Authorization: `Bearer ${response.accessToken}`
              }
            })
          );
        }),
        catchError(refreshError =>
        {
          tokenStore.removeToken();
          return throwError(() => refreshError);
        })
      );
    })
  );
};
