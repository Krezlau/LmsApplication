import { RouterConfigOptions, Routes } from '@angular/router';
import { HomePageComponent } from './components/home-page/home-page.component';
import { AdminPanelPageComponent } from './components/admin-panel-page/admin-panel-page.component';
import { ProfilePageComponent } from './components/profile-page/profile-page.component';
import { CourseDetailsPageComponent } from './components/course-details-page/course-details-page.component';
import { LoginPageComponent } from './components/login-page/login-page.component';
import { RegisterPageComponent } from './components/register-page/register-page.component';
import { EditUserProfilePageComponent } from './components/edit-user-profile-page/edit-user-profile-page.component';
import { CourseEditionPageComponent } from './components/course-edition-page/course-edition-page.component';
import { CourseEditionOverviewComponent } from './components/course-edition-overview/course-edition-overview.component';
import { CourseEditionMembersComponent } from './components/course-edition-members/course-edition-members.component';
import { CourseEditionSettingsComponent } from './components/course-edition-settings/course-edition-settings.component';
import { CourseEditionBoardComponent } from './components/course-edition-board/course-edition-board.component';
import { CourseEditionResourcesComponent } from './components/course-edition-resources/course-edition-resources.component';
import { CourseEditionGradesComponent } from './components/course-edition-grades/course-edition-grades.component';

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
    path: 'editions/:editionId',
    component: CourseEditionPageComponent,
    children: [
      {
        path: '',
        component: CourseEditionOverviewComponent,
      },
      {
        path: 'board',
        component: CourseEditionBoardComponent,
      },
      {
        path: 'members',
        component: CourseEditionMembersComponent,
      },
      {
        path: 'settings',
        component: CourseEditionSettingsComponent,
      },
      {
        path: 'resources',
        component: CourseEditionResourcesComponent,
      },
      {
        path: 'grades',
        component: CourseEditionGradesComponent,
      },
    ],
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
