import { Component, DestroyRef, inject, input, OnInit, output, signal } from '@angular/core';
import { ButtonComponent } from '../global-components/button/button.component';
import { AccountModel } from '../models/account.model';
import { UserAccountService } from '../services/useracount.service';
import { catchError, throwError } from 'rxjs';

@Component({
  selector: 'app-account-page',
  standalone: true,
  imports: [ButtonComponent],
  templateUrl: './account-page.component.html',
  styleUrl: './account-page.component.css',
})
export class AccountPageComponent implements OnInit {
  private userAccountService = inject(UserAccountService);
  private destroyRef = inject(DestroyRef);
  userAccount? : AccountModel;
  editMode  = signal<boolean>(false);
  //editMode : boolean = false

  ngOnInit() {
    const subscription = this.userAccountService
          .getUserNameAndEmail()
          .pipe(
            catchError((error) => {
              return throwError(() => new Error(error));
          })
        )
        .subscribe({
            next: (response : AccountModel) => {
              this.userAccount = response;
            },
            error: (error) => {
              console.log(error)
            }
          })
        this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  switchEditMode(){
    this.editMode.set(!this.editMode())
  }

}
