import { Component } from '@angular/core';
import { ButtonComponent } from '../global-components/button/button.component';
import { LinkButtonComponent } from '../global-components/link-button/link-button.component';

@Component({
  selector: 'app-main-page',
  standalone: true,
  imports: [ButtonComponent, LinkButtonComponent],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.css',
})
export class MainPageComponent {}
