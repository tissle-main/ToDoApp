import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { provideRouter, Routes } from '@angular/router';
import { ApplicationConfig, inject, provideAppInitializer, provideBrowserGlobalErrorListeners } from '@angular/core';
import { API_BASE_URL } from './app/api';
import { AuthTokenStore } from './app/features/auth/auth.token.store';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './app/features/auth/auth.http.interceptor';
import { RegisterPage } from './app/pages/register.page/register.page';
import { LoginPage } from './app/pages/login.page/login.page';
import { guestGuard } from './app/features/auth/guest.guard';
import { MainPage } from './app/pages/main.page/main.page';
import { authGuard } from './app/features/auth/auth.guard';

const routes: Routes = [
  {
    path: "register",
    component: RegisterPage,
    canActivate: [guestGuard]
  },
  {
    path: "login",
    component: LoginPage,
    canActivate: [guestGuard]
  },
  {
    path: "",
    component: MainPage,
    canActivate: [authGuard]
  }
];
const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideAppInitializer(() =>
    {
      inject(AuthTokenStore).restoreToken()
    }),    
    provideHttpClient(
      withInterceptors([authInterceptor])
    ),
    {
      provide: API_BASE_URL,
      useValue: "/api"
    }
  ]
};
bootstrapApplication(App, appConfig).catch(console.error);
