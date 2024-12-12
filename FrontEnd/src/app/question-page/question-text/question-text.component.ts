import { Component, input } from '@angular/core';

@Component({
  selector: 'app-question-text',
  standalone: true,
  imports: [],
  templateUrl: './question-text.component.html',
  styleUrl: './question-text.component.css',
})
export class QuestionTextComponent {
  question = input.required<string>();
}
