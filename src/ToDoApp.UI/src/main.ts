import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app';
import { provideRouter, Routes } from '@angular/router';
import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';

const routes: Routes = [];
const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    // {
    //   provide: API_BASE_URL,
    //   useValue: "/api"
    // }
  ]
};
bootstrapApplication(App, appConfig).catch(console.error);
