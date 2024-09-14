import {ROUTER_CONFIGURATION, RouterConfigOptions, RouterConfigurationFeature, Routes} from '@angular/router';
import {HomePageComponent} from "./components/home-page/home-page.component";
import {AdminPanelPageComponent} from "./components/admin-panel-page/admin-panel-page.component";
import {ProfilePageComponent} from "./components/profile-page/profile-page.component";
import {CourseDetailsPageComponent} from "./components/course-details-page/course-details-page.component";

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
    path: ':tenantId/courses/:courseId',
    component: CourseDetailsPageComponent
  },
  {
    path: ':tenantId',
    redirectTo: ':tenantId/home',
    pathMatch: 'full'
  },
  {
    path: '**',
    component: HomePageComponent
  }
];

export const routerConfig: RouterConfigOptions = {
  urlUpdateStrategy: 'eager',
};
