import { Component, DestroyRef, inject } from '@angular/core';
import { AuthorizatinService } from '../../../services/authorizationcalls.service';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { catchError, throwError } from 'rxjs';
import { ButtonComponent } from '../../../global-components/button/button.component';
import { RegisterAdminModel } from '../../../models/admin-models/registeradmin.model';
import { UserAdminService } from '../../../services/admin-services/usersadmin.service';
import { LinkButtonComponent } from '../../../global-components/link-button/link-button.component';

@Component({
  selector: 'app-register-new-user-admin-page',
  standalone: true,
  imports: [ButtonComponent, ReactiveFormsModule, LinkButtonComponent],
  templateUrl: './register-new-user-admin-page.component.html',
  styleUrl: './register-new-user-admin-page.component.css',
})
export class RegisterNewUserAdminPageComponent {
  private userAdminService = inject(UserAdminService);
  private destroyRef = inject(DestroyRef);
  private doPasswordsMatch = true;
  wasSuccess = false;
  errorMessage = '';

  registerForm = new FormGroup({
    userName: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    email: new FormControl<string>('', [Validators.required, Validators.email]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(8),
    ]),
    confirmPassword: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(8),
    ]),
    isActive: new FormControl<boolean>(true),
    isAdmin: new FormControl<boolean>(false),
  });

  Register() {
    if (
      this.CheckPassWord(
        this.registerForm.value.password,
        this.registerForm.value.confirmPassword
      )
    ) {
      const subscription = this.userAdminService
        .registerNewUser(this.constructRegisterModel)
        .pipe(
          catchError((error) => {
            return throwError(() => new Error(error));
          })
        )
        .subscribe({
          next: (response) => {
            this.wasSuccess = true;
          },
          error: (error) => {
            this.wasSuccess = false;
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

  ResetForm() {
    this.registerForm.setValue({
      userName: '',
      email: '',
      password: '',
      confirmPassword: '',
      isActive: true,
      isAdmin: false,
    });
    (this.errorMessage = ''), (this.wasSuccess = false);
  }

  get constructRegisterModel() {
    let registerModel: RegisterAdminModel = {
      UserName: this.registerForm.value.userName!,
      Email: this.registerForm.value.email!,
      Password: this.registerForm.value.password!,
      IsActive: this.registerForm.value.isActive! === true ? 1 : 0,
      IsAdmin: this.registerForm.value.isAdmin! === true ? 1 : 0,
    };
    return registerModel;
  }

  get getDoPasswordsMatch() {
    return this.doPasswordsMatch;
  }
}
