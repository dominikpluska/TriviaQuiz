import { Component } from '@angular/core';
import { ButtonComponent } from '../../global-components/button/button.component';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LinkButtonComponent } from '../../global-components/link-button/link-button.component';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ButtonComponent, ReactiveFormsModule, LinkButtonComponent],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css',
})
export class LoginPageComponent {
  loginForm = new FormGroup({
    login: new FormControl(''),
    password: new FormControl(''),
  });

  LogActoin() {
    console.log(this.loginForm.value.login);
  }
}
