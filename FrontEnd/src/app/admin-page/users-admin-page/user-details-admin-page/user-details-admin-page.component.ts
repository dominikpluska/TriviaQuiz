import { Component, DestroyRef, inject, input, OnInit } from '@angular/core';
import { User } from '../../../models/admin-models/user.model';
import { catchError, throwError } from 'rxjs';
import { ButtonComponent } from '../../../global-components/button/button.component';
import { LinkButtonComponent } from '../../../global-components/link-button/link-button.component';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { UserAdminService } from '../../../services/admin-services/usersadmin.service';

@Component({
  selector: 'app-user-details-admin-page',
  standalone: true,
  imports: [LinkButtonComponent, ReactiveFormsModule, ButtonComponent],
  templateUrl: './user-details-admin-page.component.html',
  styleUrl: './user-details-admin-page.component.css',
})
export class UserDetailsAdminPageComponent implements OnInit {
  userId = input.required<number>();
  user!: User;
  errorMessage?: string;
  disabled: boolean = true;
  wasSuccess: boolean = false;
  changePasswordMode: boolean = false;
  private usersService = inject(UserAdminService);
  private destroyRef = inject(DestroyRef);

  userForm = new FormGroup({
    userName: new FormControl<string>(
      { value: this.user?.userName, disabled: this.disabled },
      [Validators.required, Validators.minLength(5)]
    ),

    email: new FormControl<string>(
      { value: this.user?.email, disabled: this.disabled },
      [Validators.required, Validators.email]
    ),
    isActive: new FormControl<boolean>(
      {
        value: this.user?.isActive === 1 ? true : false,
        disabled: this.disabled,
      },
      [Validators.required, Validators.email]
    ),
    isAdmin: new FormControl<boolean>(
      {
        value: this.user?.isGameMaster === 1 ? true : false,
        disabled: this.disabled,
      },
      [Validators.required, Validators.email]
    ),
  });

  passWordForm = new FormGroup({
    password: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5),
    ]),

    confirmPassword: new FormControl<string>('', [
      Validators.required,
      Validators.email,
    ]),
  });

  ngOnInit() {
    if (this.userId() === null) {
      this.errorMessage = 'UserId is empty!';
    } else {
      this.FetchUser();
    }
  }

  switchEditMode() {
    this.errorMessage = '';
    this.disabled = !this.disabled;
    if (this.disabled === false) {
      this.userForm.controls['userName'].enable();
      this.userForm.controls['email'].enable();
      this.userForm.controls['isActive'].enable();
      this.userForm.controls['isAdmin'].enable();
    } else {
      this.userForm.controls['userName'].disable();
      this.userForm.controls['email'].disable();
      this.userForm.controls['isActive'].disable();
      this.userForm.controls['isAdmin'].disable();
    }
  }

  resetForm() {
    this.userForm.setValue({
      userName: this.user.userName,
      email: this.user.email,
      isActive: this.user.isActive === 1 ? true : false,
      isAdmin: this.user.isGameMaster === 1 ? true : false,
    });
    this.switchEditMode();
    this.errorMessage = '';
  }

  handleUpdate() {
    const subscription = this.usersService
      .updateUser({
        userId: this.user.userId,
        userName: this.userForm.value.userName!,
        email: this.userForm.value.email!,
        isActive: this.userForm.value.isActive! === true ? 1 : 0,
        isGameMaster: this.userForm.value.isAdmin! === true ? 1 : 0,
      })
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response) => {
          this.wasSuccess = true;
          this.errorMessage = '';
        },
        error: (error) => {
          this.errorMessage = error;
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  switchChangePasswordMode() {
    this.changePasswordMode = !this.changePasswordMode;
    this.errorMessage = '';
    this.passWordForm.setValue({ password: '', confirmPassword: '' });
  }

  handleChangePassWord() {
    if (this.CheckPasswords()) {
      const subscription = this.usersService
        .updatePassword(this.user.userId, this.passWordForm.value.password!)
        .pipe(
          catchError((error) => {
            return throwError(() => new Error(error));
          })
        )
        .subscribe({
          next: (response) => {
            this.wasSuccess = true;
            this.errorMessage = '';
            this.passWordForm.setValue({ password: '', confirmPassword: '' });
          },
          error: (error) => {
            this.errorMessage = error;
            this.wasSuccess = false;
          },
        });
      this.destroyRef.onDestroy(() => subscription.unsubscribe());
    }
  }

  handleDeleteUser() {
    const subscription = this.usersService
      .deleteUser(this.user.userId)
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response) => {
          this.wasSuccess = true;
          this.errorMessage = '';
        },
        error: (error) => {
          this.errorMessage = error;
          this.wasSuccess = false;
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  ResetComponent() {
    this.FetchUser();
    this.wasSuccess = false;
    this.errorMessage = '';
  }

  private FetchUser() {
    const subscription = this.usersService
      .getUser(this.userId())
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response: User) => {
          this.userForm.setValue({
            userName: response.userName,
            email: response.email,
            isActive: response.isActive === 1 ? true : false,
            isAdmin: response.isGameMaster === 1 ? true : false,
          });
          this.user = response;
        },
        error: (error) => {
          this.errorMessage = error;
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  private CheckPasswords() {
    if (
      this.passWordForm.value.password! ==
      this.passWordForm.value.confirmPassword
    ) {
      this.errorMessage = '';
      return true;
    } else if (this.passWordForm.value.password!.length < 5) {
      this.errorMessage = 'Password is too short!';
      return false;
    } else {
      this.errorMessage = 'Passwords do not match!';
      return false;
    }
  }
}
