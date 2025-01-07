import { Component, DestroyRef, inject, output } from '@angular/core';
import { ButtonComponent } from "../../global-components/button/button.component";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LinkButtonComponent } from "../../global-components/link-button/link-button.component";
import { UserAccountService } from '../../services/useracount.service';
import { catchError, throwError } from 'rxjs';
import {PasswordChange} from '../../models/passwordchange.model';

@Component({
  selector: 'app-change-password-form',
  standalone: true,
  imports: [ButtonComponent, ReactiveFormsModule, LinkButtonComponent],
  templateUrl: './change-password-form.component.html',
  styleUrl: './change-password-form.component.css'
})
export class ChangePasswordFormComponent {
      private userAccountService = inject(UserAccountService);
      private destroyRef = inject(DestroyRef);
      private errorMessage : string = '';
      goBackClick = output<void>();
      wasSuccessful : boolean = false;
      
      passwordForm = new FormGroup({
        oldPassword: new FormControl<string>('',[Validators.required, Validators.minLength(5)]),
        newPassword: new FormControl<string>('', [Validators.required, Validators.minLength(5)]),
        confirmNewPassword: new FormControl<string>('',[Validators.required, Validators.minLength(5)]),
      });

      submitNewPassword(){
        if(this.checkPasswordIntegrity() && this.checkPasswordLength()){
          let newPassword = this.constructPasswordModel()
          const subscription = this.userAccountService
                    .changePassword(newPassword)
                    .pipe(
                      catchError((error) => {
                        return throwError(() => new Error(error));
                    })
                  )
                  .subscribe({
                      next: (response) => {
                        this.wasSuccessful = true;
                      },
                      error: (error) => {
                        this.errorMessage = error;
                        //console.log(error)
                      }
                    })
          this.destroyRef.onDestroy(() => subscription.unsubscribe());
        }

      }

      private checkPasswordIntegrity(){
        let newPasswordCheck = this.passwordForm.get('newPassword')?.value
        let confirmNewPasswordCheck = this.passwordForm.get('confirmNewPassword')?.value
        if(newPasswordCheck === confirmNewPasswordCheck){
          return true;
        }
        else{
          this.errorMessage = 'Passwords do not match!'
          return false
        }
      }

      private checkPasswordLength(){
        let newPasswordCheck = this.passwordForm.get('newPassword')?.value
        if(newPasswordCheck!.length < 5){
          this.errorMessage = 'Password is too short!'
          return false
        }
        else{
          return true;
        }
      }

      private constructPasswordModel(){
        let oldPassword = this.passwordForm.get('oldPassword')?.value!
        let newPassword = this.passwordForm.get('newPassword')?.value!
        let confirmNewPassword = this.passwordForm.get('confirmNewPassword')?.value!

        let passwordChangeModel : PasswordChange = {
          oldPassword: oldPassword, 
          newPassword: newPassword, 
          newPasswordConfirm: confirmNewPassword
        }

        return passwordChangeModel;
      }

      get getError(){
        return this.errorMessage;
      }

      goBack() {
        this.goBackClick.emit();
      }
}
