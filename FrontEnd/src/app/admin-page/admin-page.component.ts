import { Component } from '@angular/core';
import { LinkButtonComponent } from "../global-components/link-button/link-button.component";

@Component({
  selector: 'app-admin-page',
  standalone: true,
  imports: [LinkButtonComponent],
  templateUrl: './admin-page.component.html',
  styleUrl: './admin-page.component.css'
})
export class AdminPageComponent {

}
