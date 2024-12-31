import { Component, input, output } from '@angular/core';

@Component({
  selector: 'app-question-button',
  standalone: true,
  imports: [],
  templateUrl: './question-button.component.html',
  styleUrl: './question-button.component.css',
})
export class QuestionButtonComponent {
  answer = input.required<string>();
  select = output<string>();

  onSelect(){
    this.select.emit(this.answer());
  }

}
