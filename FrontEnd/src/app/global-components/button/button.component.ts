import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [],
  templateUrl: './button.component.html',
  styleUrl: './button.component.css',
})
export class ButtonComponent {
  content = input.required<string>();
  buttonClass = input.required<'main' | 'secondary' | 'delete'>();
  buttonClick = output<void>();

  onButtonLcikc() {
    this.buttonClick.emit();
  }
}
