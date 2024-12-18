import { Component, DestroyRef, inject } from '@angular/core';
import { ButtonComponent } from '../../global-components/button/button.component';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { RegisterModel } from '../../models/register.model';
import { AuthorizatinService } from '../../services/authorizationcalls.service';
import { RegistrationSuccessComponent } from './registration-success/registration-success.component';
import { RegistrationErrorComponent } from './registration-error/registration-error.component';
import { catchError, throwError } from 'rxjs';

@Component({
  selector: 'app-registration-page',
  standalone: true,
  imports: [
    ButtonComponent,
    ReactiveFormsModule,
    RegistrationSuccessComponent,
    RegistrationErrorComponent,
  ],
  templateUrl: './registration-page.component.html',
  styleUrl: './registration-page.component.css',
})
export class RegistrationPageComponent {
  private registerApiCall = inject(AuthorizatinService);
  private destroyRef = inject(DestroyRef);
  private doPasswordsMatch = true;
  private requestSent = false;
  private wasError = false;
  errorMessage = '';

  registerForm = new FormGroup({
    userName: new FormControl('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    email: new FormControl('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8),
    ]),
    confirmPassword: new FormControl('', [
      Validators.required,
      Validators.minLength(8),
    ]),
  });

  Register() {
    if (
      this.CheckPassWord(
        this.registerForm.value.password,
        this.registerForm.value.confirmPassword
      )
    ) {
      const subscription = this.registerApiCall
        .createAccount(this.constructRegisterModel)
        .pipe(
          catchError((error) => {
            return throwError(() => new Error(error));
          })
        )
        .subscribe({
          next: (response) => {
            this.requestSent = true;
            console.log('Success:');
          },
          error: (error) => {
            this.requestSent = true;
            this.wasError = true;
            this.errorMessage = error;
          },
        });
      this.destroyRef.onDestroy(() => subscription.unsubscribe());
    } else {
      return;
    }
  }

  CheckPassWord(
    password: string | null | undefined,
    confirmPassword: string | null | undefined
  ) {
    if (password === confirmPassword) {
      return true;
    } else {
      this.doPasswordsMatch = false;
      return false;
    }
  }
  setPasswordInputStyle() {
    if (this.doPasswordsMatch) {
      return `mb-2 w-96 px-2 py-2 rounded-lg bg-slate-300 text-black border-2 
      border-gray-800 focus:border-slate-900 focus:outline-none 
      focus:bg-slate-500 `;
    } else {
      return `mb-2 w-96 px-2 py-2 rounded-lg bg-red-300 text-black border-2 
      border-red-800 focus:border-red-900 focus:outline-none 
      focus:bg-slate-500 w-3/5`;
    }
  }

  get constructRegisterModel() {
    let registerModel: RegisterModel = {
      UserName: this.registerForm.value.userName,
      Email: this.registerForm.value.email,
      Password: this.registerForm.value.password,
    };
    return registerModel;
  }

  get getDoPasswordsMatch() {
    return this.doPasswordsMatch;
  }

  get getRequestSent() {
    return this.requestSent;
  }
  get getWasError() {
    return this.wasError;
  }
}
