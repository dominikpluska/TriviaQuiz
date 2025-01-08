import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { LinkButtonComponent } from '../../global-components/link-button/link-button.component';
import { UsersService } from '../../services/admin-services/users.service';
import { catchError, throwError } from 'rxjs';
import { User } from '../../models/admin-models/user.model';
import { BoolTransformerPipe } from '../../custom-pipes/booltransformer.pipe';
import { Router } from '@angular/router';

@Component({
  selector: 'app-users-admin-page',
  standalone: true,
  imports: [LinkButtonComponent, BoolTransformerPipe],
  templateUrl: './users-admin-page.component.html',
  styleUrl: './users-admin-page.component.css',
})
export class UsersAdminPageComponent implements OnInit {
  private usersService = inject(UsersService);
  private destroyRef = inject(DestroyRef);
  private router = inject(Router);
  userList!: User[];

  ngOnInit(): void {
    const subscription = this.usersService
      .getAllUsers()
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response: User[]) => {
          this.userList = response;
          console.log(response);
        },
        error: (error) => {
          console.log(error);
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  openUserDetails(userId: number) {
    this.router.navigate(['admin/users/details'], {
      queryParams: { userId: userId },
    });
  }
}
