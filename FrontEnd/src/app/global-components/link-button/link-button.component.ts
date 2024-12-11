import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-link-button',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './link-button.component.html',
  styleUrl: './link-button.component.css',
})
export class LinkButtonComponent {
  content = input.required<string>();
  link = input.required<string>();
}
