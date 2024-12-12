import { Component } from '@angular/core';
import { QuestionButtonComponent } from './question-button/question-button.component';
import { QuestionTextComponent } from './question-text/question-text.component';

@Component({
  selector: 'app-question-page',
  standalone: true,
  imports: [QuestionButtonComponent, QuestionTextComponent],
  templateUrl: './question-page.component.html',
  styleUrl: './question-page.component.css',
})
export class QuestionPageComponent {}
