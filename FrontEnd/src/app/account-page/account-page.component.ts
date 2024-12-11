import { Component, input } from '@angular/core';
import { ButtonComponent } from '../global-components/button/button.component';

@Component({
  selector: 'app-account-page',
  standalone: true,
  imports: [ButtonComponent],
  templateUrl: './account-page.component.html',
  styleUrl: './account-page.component.css',
})
export class AccountPageComponent {
  userName = 'test';
  email = 'test@test.com';
}
