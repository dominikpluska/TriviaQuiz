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
  //buttonType = input<string>();
  buttonClass = input.required<'main' | 'secondary'>();
  buttonClick = output<void>();

  onButtonLcikc() {
    this.buttonClick.emit();
  }
}
