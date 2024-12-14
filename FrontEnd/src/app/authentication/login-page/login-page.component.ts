import { Component, DestroyRef, inject } from '@angular/core';
import { ButtonComponent } from '../../global-components/button/button.component';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { LinkButtonComponent } from '../../global-components/link-button/link-button.component';
import { AuthorizatinService } from '../../services/authorizationcalls.service';
import { LoginModel } from '../../models/login.model';
import { catchError, throwError } from 'rxjs';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [ButtonComponent, ReactiveFormsModule, LinkButtonComponent],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css',
})
export class LoginPageComponent {
  private loginApiCall = inject(AuthorizatinService);
  private destroyRef = inject(DestroyRef);
  private errorMessage = '';

  loginForm = new FormGroup({
    login: new FormControl(''),
    password: new FormControl(''),
  });

  LogActoin() {
    const subscription = this.loginApiCall
      .logIn(this.constructLoginModel)
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response) => {
          this.errorMessage = '';
          console.log('Success:', response);
        },
        error: (error) => {
          this.errorMessage = error;
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  get constructLoginModel() {
    let registerModel: LoginModel = {
      userName: this.loginForm.value.login,
      password: this.loginForm.value.password,
    };
    return registerModel;
  }

  setInputStyle() {
    if (this.errorMessage.length >= 0) {
      return `mb-2 w-96 px-2 py-2 rounded-lg bg-slate-300 text-black border-2 
      border-gray-800 focus:border-slate-900 focus:outline-none 
      focus:bg-slate-500 w-3/5`;
    } else {
      return `mb-2 w-96 px-2 py-2 rounded-lg bg-red-300 text-black border-2 
      border-red-800 focus:border-red-900 focus:outline-none 
      focus:bg-red-500 w-3/5`;
    }
  }

  get getError() {
    return this.errorMessage;
  }
}
