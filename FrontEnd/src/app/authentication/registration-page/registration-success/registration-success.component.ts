import { Component } from '@angular/core';
import { LinkButtonComponent } from '../../../global-components/link-button/link-button.component';

@Component({
  selector: 'app-registration-success',
  standalone: true,
  imports: [LinkButtonComponent],
  templateUrl: './registration-success.component.html',
  styleUrl: './registration-success.component.css',
})
export class RegistrationSuccessComponent {}
