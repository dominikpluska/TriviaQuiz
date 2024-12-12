import { HttpClient } from '@angular/common/http';
import { DestroyRef, inject, Injectable } from '@angular/core';
import { RegisterModel } from '../models/register.model';

@Injectable({ providedIn: 'root' })
export class AuthorizatinService {
  private htppClient: HttpClient = inject(HttpClient);
  private destroyRef = inject(DestroyRef);

  createAccount(registerModel: RegisterModel) {
    let UserDto = JSON.stringify(registerModel);
    const subscription = this.htppClient
      .post('https://localhost:7501/Register', UserDto)
      .subscribe({
        next: () => console.log('response'),
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }
}
