import { Component, DestroyRef, inject } from '@angular/core';
import { QuestionsAdminService } from '../../../services/admin-services/questionsadmin.service';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ButtonComponent } from '../../../global-components/button/button.component';
import { LinkButtonComponent } from '../../../global-components/link-button/link-button.component';
import { QuestionDetails } from '../../../models/admin-models/questiondetails.model';
import { catchError, throwError } from 'rxjs';

@Component({
  selector: 'app-register-new-question-admin-page',
  standalone: true,
  imports: [ButtonComponent, LinkButtonComponent, ReactiveFormsModule],
  templateUrl: './register-new-question-admin-page.component.html',
  styleUrl: './register-new-question-admin-page.component.css',
})
export class RegisterNewQuestionAdminPageComponent {
  private questionsAdminService = inject(QuestionsAdminService);
  private destroyRef = inject(DestroyRef);
  errorMessage?: string;
  wasSuccess: boolean = false;

  questionDetailsForm = new FormGroup({
    questionTitle: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    questionDescription: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    questionCategory: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    optionA: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    optionB: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    optionC: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    optionD: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    correctAnswer: new FormControl<string>('', [
      Validators.required,
      Validators.minLength(5),
    ]),
    questionScore: new FormControl<number>(5),
  });

  handlePost() {
    const subscription = this.questionsAdminService
      .postQuestion(this.constructQuestionDetailsModel())
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response) => {
          this.errorMessage = '';
          this.wasSuccess = true;
          this.clearForm();
        },
        error: (error) => {
          this.errorMessage = error;
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }
  resetForm() {
    this.clearForm();
    (this.errorMessage = ''), (this.wasSuccess = false);
  }

  private constructQuestionDetailsModel() {
    let question: QuestionDetails = {
      questionCategory: this.questionDetailsForm.value.questionCategory!,
      questionDescription: this.questionDetailsForm.value.questionDescription!,
      questionScore: this.questionDetailsForm.value.questionScore!,
      questionTitle: this.questionDetailsForm.value.questionTitle!,
      optionA: this.questionDetailsForm.value.optionA!,
      optionB: this.questionDetailsForm.value.optionB!,
      optionC: this.questionDetailsForm.value.optionC!,
      optionD: this.questionDetailsForm.value.optionD!,
      correctAnswer: this.questionDetailsForm.value.correctAnswer!,
    };

    return question;
  }
  private clearForm() {
    this.questionDetailsForm.setValue({
      questionCategory: '',
      questionDescription: '',
      questionTitle: '',
      questionScore: 5,
      optionA: '',
      optionB: '',
      optionC: '',
      optionD: '',
      correctAnswer: '',
    });
  }
}
