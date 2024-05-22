import { Routes } from '@angular/router';
import {HomePageComponent} from "./components/home-page/home-page.component";

export const routes: Routes = [
  {
    path: ':tenantId/home',
    component: HomePageComponent,
  },
  {
    path: '',
    redirectTo: 'tenant1/home',
    pathMatch: 'full'
  },
  {
    path: ':tenantId/admin',
    component: HomePageComponent
  },
  {
    path: '**',
    component: HomePageComponent
  }
];
