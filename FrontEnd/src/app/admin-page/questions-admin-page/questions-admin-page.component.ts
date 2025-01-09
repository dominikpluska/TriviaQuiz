import { Component } from '@angular/core';
import { LinkButtonComponent } from '../../global-components/link-button/link-button.component';

@Component({
  selector: 'app-questions-admin-page',
  standalone: true,
  imports: [LinkButtonComponent],
  templateUrl: './questions-admin-page.component.html',
  styleUrl: './questions-admin-page.component.css',
})
export class QuestionsAdminPageComponent {}
