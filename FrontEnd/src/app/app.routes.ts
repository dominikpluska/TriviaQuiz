import { Routes } from '@angular/router';
import { Component } from '@angular/core';
import { LoginPageComponent } from './authentication/login-page/login-page.component';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginPageComponent,
  },
];
