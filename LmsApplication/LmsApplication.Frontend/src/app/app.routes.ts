import { Routes } from '@angular/router';
import {HomePageComponent} from "./components/home-page/home-page.component";
import {AdminPanelPageComponent} from "./components/admin-panel-page/admin-panel-page.component";
import {ProfilePageComponent} from "./components/profile-page/profile-page.component";

export const routes: Routes = [
  {
    path: ':tenantId/home',
    component: HomePageComponent,
  },
  {
    path: ':tenantId/admin',
    component: AdminPanelPageComponent
  },
  {
    path: ':tenantId/users/:email',
    component: ProfilePageComponent
  },
  {
    path: '',
    redirectTo: 'tenant1/home',
    pathMatch: 'full'
  },
  {
    path: '**',
    component: HomePageComponent
  }
];
