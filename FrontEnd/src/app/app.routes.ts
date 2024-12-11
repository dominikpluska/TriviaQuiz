import { Routes } from '@angular/router';
import { LoginPageComponent } from './authentication/login-page/login-page.component';
import { RegistrationPageComponent } from './authentication/registration-page/registration-page.component';
import { MainPageComponent } from './main-page/main-page.component';
import { NotFoundPageComponent } from './not-found-page/not-found-page.component';
import { AccountPageComponent } from './account-page/account-page.component';

export const routes: Routes = [
  {
    path: '*',
    component: NotFoundPageComponent,
  },
  {
    path: 'main',
    component: MainPageComponent,
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
    path: 'account',
    component: AccountPageComponent,
  },
];
