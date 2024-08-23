import { ApplicationConfig, NgModule } from '@angular/core';
import { RouterModule, provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { CommonModule } from '@angular/common';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';

export const appConfig: ApplicationConfig = {
  providers: [provideRouter(routes), provideAnimationsAsync('noop')]
};
