import { Component, DestroyRef, inject, input, OnInit } from '@angular/core';
import { User } from '../../../models/admin-models/user.model';
import { UsersService } from '../../../services/admin-services/users.service';
import { catchError, throwError } from 'rxjs';
import { ButtonComponent } from '../../../global-components/button/button.component';
import { LinkButtonComponent } from '../../../global-components/link-button/link-button.component';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

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
  private usersService = inject(UsersService);
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

  ngOnInit() {
    if (this.userId() === null) {
      this.errorMessage = 'UserId is empty!';
    } else {
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
  }

  switchEditMode() {
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
  }
}
