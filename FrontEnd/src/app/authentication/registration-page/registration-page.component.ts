import { Component } from '@angular/core';
import { ButtonComponent } from '../../global-components/button/button.component';

@Component({
  selector: 'app-registration-page',
  standalone: true,
  imports: [ButtonComponent],
  templateUrl: './registration-page.component.html',
  styleUrl: './registration-page.component.css',
})
export class RegistrationPageComponent {
  LogAction() {
    console.log('test');
  }
}
