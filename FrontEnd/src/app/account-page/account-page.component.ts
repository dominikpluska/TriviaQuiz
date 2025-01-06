import { Component, DestroyRef, inject, input, OnInit, output, signal } from '@angular/core';
import { AccountModel } from '../models/account.model';
import { UserAccountService } from '../services/useracount.service';
import { catchError, throwError } from 'rxjs';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LinkButtonComponent } from "../global-components/link-button/link-button.component";
import { ButtonComponent } from "../global-components/button/button.component";
import { ChangePasswordFormComponent } from './change-password-form/change-password-form.component';

@Component({
  selector: 'app-account-page',
  standalone: true,
  imports: [ReactiveFormsModule, LinkButtonComponent, ButtonComponent, ChangePasswordFormComponent],
  templateUrl: './account-page.component.html',
  styleUrl: './account-page.component.css',
})
export class AccountPageComponent implements OnInit {
  private userAccountService = inject(UserAccountService);
  private destroyRef = inject(DestroyRef);
  private errorMessage : string = '';
  private userAccount! : AccountModel;
  displayChangePasswordForm = signal<boolean>(false);
  wasSuccessful: boolean = false;
  disabled : boolean = true;

    accountForm = new FormGroup({
      userName: new FormControl<string>(
        {value: this.userAccount?.userName, disabled : this.disabled}, 
        [Validators.required, Validators.minLength(5)]),
        
      email: new FormControl<string>(
        {value: this.userAccount?.email, disabled : this.disabled},
        [Validators.required, Validators.email]),
    });

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
              this.accountForm.setValue({userName : response.userName, email: response.email});
              this.userAccount = response;
            },
            error: (error) => {
              console.log(error)
            }
          })
        this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  switchEditMode(){
    this.disabled = !this.disabled;
    if(this.disabled === false){
      this.accountForm.controls['userName'].enable();
      this.accountForm.controls['email'].enable();
    }
    else{
      this.accountForm.controls['userName'].disable();
      this.accountForm.controls['email'].disable();
    }
  }

  handleChangeUserNameAndEmail(){
    let userName = this.accountForm.get('userName')!.value!
    let email = this.accountForm.get('email')!.value!

    if(userName === '' || email && '')
    {
      this.errorMessage = 'UserName or Email cannot be empty!';
      return
    }

    const subscription = this.userAccountService
      .updateUserNameAndEmail({userName : userName, email : email})
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response) => {
          console.log(response)
          this.errorMessage = '';
          this.switchEditMode();
          this.wasSuccessful = true;
          //this.router.navigate(['/main']);
        },
        error: (error) => {
          console.log(error);
          this.errorMessage = error;
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  resetForm(){
    this.accountForm.setValue({userName : this.userAccount?.userName, email: this.userAccount?.email});
    this.switchEditMode();
  }

  changeDisplayChangePasswordForm(){
    this.displayChangePasswordForm.set(!this.displayChangePasswordForm());
  }

  get getError() {
    return this.errorMessage;
  }
}
