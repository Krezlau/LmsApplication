import { RouterConfigOptions, Routes } from '@angular/router';
import { HomePageComponent } from './components/home-page/home-page.component';
import { AdminPanelPageComponent } from './components/admin-panel-page/admin-panel-page.component';
import { ProfilePageComponent } from './components/profile-page/profile-page.component';
import { CourseDetailsPageComponent } from './components/course-details-page/course-details-page.component';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { RegisterPageComponent } from './components/register-page/register-page.component';
import { EditUserProfilePageComponent } from './components/edit-user-profile-page/edit-user-profile-page.component';

export const routes: Routes = [
  {
    path: 'home',
    component: HomePageComponent,
  },
  {
    path: 'admin',
    component: AdminPanelPageComponent,
  },
  {
    path: 'login',
    component: LoginPageComponent,
  },
  {
    path: 'register',
    component: RegisterPageComponent,
  },
  {
    path: 'users/:email',
    component: ProfilePageComponent,
  },
  {
    path: 'users/:email/edit',
    component: EditUserProfilePageComponent,
  },
  {
    path: 'courses/:courseId',
    component: CourseDetailsPageComponent,
  },
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: '**',
    component: HomePageComponent,
  },
];

export const routerConfig: RouterConfigOptions = {
  urlUpdateStrategy: 'eager',
};
