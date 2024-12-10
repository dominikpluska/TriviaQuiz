import { Component, input } from '@angular/core';
import { InputComponent } from '../../global-components/input/input.component';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [InputComponent],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css',
})
export class LoginPageComponent {}
