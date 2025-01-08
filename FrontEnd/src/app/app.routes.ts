import { Routes } from '@angular/router';
import { LoginPageComponent } from './authentication/login-page/login-page.component';
import { RegistrationPageComponent } from './authentication/registration-page/registration-page.component';
import { MainPageComponent } from './main-page/main-page.component';
import { NotFoundPageComponent } from './not-found-page/not-found-page.component';
import { AccountPageComponent } from './account-page/account-page.component';
import { QuestionPageComponent } from './question-page/question-page.component';
import { StatsPageComponent } from './stats-page/stats-page.component';
import { AuthGuard } from './services/authguard.service';
import { StatsDetailsPageComponent } from './stats-page/stats-details-page/stats-details-page.component';
import { AdminPageComponent } from './admin-page/admin-page.component';
import { AuthGuardAdmin } from './services/authguardadmin.service';
import { UsersAdminPageComponent } from './admin-page/users-admin-page/users-admin-page.component';
import { QuestionsAdminPageComponent } from './admin-page/questions-admin-page/questions-admin-page.component';
import { UserDetailsAdminPageComponent } from './admin-page/users-admin-page/user-details-admin-page/user-details-admin-page.component';
import { RegisterNewUserAdminPageComponent } from './admin-page/users-admin-page/register-new-user-admin-page/register-new-user-admin-page.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: '/login',
    pathMatch: 'full',
  },
  {
    path: 'login',
    component: LoginPageComponent,
  },
  {
    path: 'register',
    component: RegistrationPageComponent,
  },
  {
    path: '*',
    component: NotFoundPageComponent,
  },
  {
    path: 'main',
    component: MainPageComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'account',
    component: AccountPageComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'game',
    component: QuestionPageComponent,
    canActivate: [AuthGuard],
  },
  {
    path: 'stats',
    children: [
      {
        path: '',
        component: StatsPageComponent,
      },
      {
        path: 'details',
        component: StatsDetailsPageComponent,
      },
    ],
    canActivate: [AuthGuard],
  },
  {
    path: 'admin',
    children: [
      {
        path: '',
        component: AdminPageComponent,
      },
      {
        path: 'users',
        children: [
          {
            path: '',
            component: UsersAdminPageComponent,
          },
          {
            path: 'details',
            component: UserDetailsAdminPageComponent,
          },
          {
            path: 'register',
            component: RegisterNewUserAdminPageComponent,
          },
        ],
      },
      {
        path: 'questions',
        component: QuestionsAdminPageComponent,
      },
    ],
    canActivate: [AuthGuardAdmin],
  },
];
