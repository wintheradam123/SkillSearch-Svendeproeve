import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import angular from 'angular';
import { AppModule } from './app/app.module';

angular.module('app', []);
platformBrowserDynamic()
  .bootstrapModule(AppModule)
  .catch((err) => console.error(err));
