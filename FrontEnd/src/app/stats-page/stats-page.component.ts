import { Component } from '@angular/core';
import { ButtonComponent } from '../global-components/button/button.component';
import { LinkButtonComponent } from '../global-components/link-button/link-button.component';

@Component({
  selector: 'app-stats-page',
  standalone: true,
  imports: [ButtonComponent, LinkButtonComponent],
  templateUrl: './stats-page.component.html',
  styleUrl: './stats-page.component.css',
})
export class StatsPageComponent {}
