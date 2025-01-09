import { Component, DestroyRef, inject, input, OnInit } from '@angular/core';
import { QuestionsAdminService } from '../../../services/admin-services/questionsadmin.service';
import { catchError, throwError } from 'rxjs';
import { QuestionDetails } from '../../../models/admin-models/questiondetails.model';
import {
  FormControl,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ButtonComponent } from '../../../global-components/button/button.component';
import { LinkButtonComponent } from '../../../global-components/link-button/link-button.component';

@Component({
  selector: 'app-question-details-admin-page',
  standalone: true,
  imports: [ReactiveFormsModule, ButtonComponent, LinkButtonComponent],
  templateUrl: './question-details-admin-page.component.html',
  styleUrl: './question-details-admin-page.component.css',
})
export class QuestionDetailsAdminPageComponent implements OnInit {
  private questionsAdminService = inject(QuestionsAdminService);
  private destroyRef = inject(DestroyRef);
  questionId = input.required<number>();
  questionDetails!: QuestionDetails;
  errorMessage?: string;
  disabled: boolean = true;
  wasSuccess: boolean = false;
  deleteAction: boolean = false;

  questionDetailsForm = new FormGroup({
    questionTitle: new FormControl<string>(
      { value: '', disabled: this.disabled },
      [Validators.required, Validators.minLength(5)]
    ),
    questionDescription: new FormControl<string>(
      {
        value: '',
        disabled: this.disabled,
      },
      [Validators.required, Validators.minLength(5)]
    ),
    questionCategory: new FormControl<string>(
      {
        value: '',
        disabled: this.disabled,
      },
      [Validators.required, Validators.minLength(5)]
    ),
    optionA: new FormControl<string>(
      {
        value: '',
        disabled: this.disabled,
      },
      [Validators.required, Validators.minLength(5)]
    ),
    optionB: new FormControl<string>(
      {
        value: '',
        disabled: this.disabled,
      },
      [Validators.required, Validators.minLength(5)]
    ),
    optionC: new FormControl<string>(
      {
        value: '',
        disabled: this.disabled,
      },
      [Validators.required, Validators.minLength(5)]
    ),
    optionD: new FormControl<string>(
      {
        value: '',
        disabled: this.disabled,
      },
      [Validators.required, Validators.minLength(5)]
    ),
    correctAnswer: new FormControl<string>(
      {
        value: '',
        disabled: this.disabled,
      },
      [Validators.required, Validators.minLength(5)]
    ),
    questionScore: new FormControl<number>({
      value: 0,
      disabled: this.disabled,
    }),
  });

  ngOnInit(): void {
    if (this.questionId() === null) {
      this.errorMessage = 'QuestionId must not be empty!';
    } else {
      this.fetchQuestion();
    }
  }

  switchEditMode() {
    this.errorMessage = '';
    this.disabled = !this.disabled;
    if (this.disabled === false) {
      this.questionDetailsForm.controls['questionTitle'].enable();
      this.questionDetailsForm.controls['questionDescription'].enable();
      this.questionDetailsForm.controls['questionCategory'].enable();
      this.questionDetailsForm.controls['optionA'].enable();
      this.questionDetailsForm.controls['optionB'].enable();
      this.questionDetailsForm.controls['optionC'].enable();
      this.questionDetailsForm.controls['optionD'].enable();
      this.questionDetailsForm.controls['correctAnswer'].enable();
      this.questionDetailsForm.controls['questionScore'].enable();
    } else {
      this.questionDetailsForm.controls['questionTitle'].disable();
      this.questionDetailsForm.controls['questionDescription'].disable();
      this.questionDetailsForm.controls['questionCategory'].disable();
      this.questionDetailsForm.controls['optionA'].disable();
      this.questionDetailsForm.controls['optionB'].disable();
      this.questionDetailsForm.controls['optionC'].disable();
      this.questionDetailsForm.controls['optionD'].disable();
      this.questionDetailsForm.controls['correctAnswer'].disable();
      this.questionDetailsForm.controls['questionScore'].disable();
    }
  }

  resetForm() {
    this.fetchQuestion();
    this.switchEditMode();
    this.wasSuccess = false;
    this.errorMessage = '';
  }

  handleUpdate() {
    const subscription = this.questionsAdminService
      .updateQuestion(this.constructQuestionDetailsModel())
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response) => {
          this.errorMessage = '';
          this.wasSuccess = true;
        },
        error: (error) => {
          this.errorMessage = error;
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  handleDelete() {
    const subscription = this.questionsAdminService
      .deleteQuestion(this.questionId())
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response) => {
          this.errorMessage = '';
          this.wasSuccess = true;
          this.deleteAction = true;
        },
        error: (error) => {
          this.errorMessage = error;
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  private constructQuestionDetailsModel() {
    let question: QuestionDetails = {
      questionId: this.questionDetails.questionId,
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

  private fetchQuestion() {
    const subscription = this.questionsAdminService
      .getQuestionDetails(this.questionId())
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response: QuestionDetails) => {
          this.questionDetailsForm!.setValue({
            questionCategory: response.questionCategory!,
            questionDescription: response.questionDescription,
            questionScore: response.questionScore,
            questionTitle: response.questionTitle,
            optionA: response.optionA,
            optionB: response.optionB,
            optionC: response.optionC,
            optionD: response.optionD,
            correctAnswer: response.correctAnswer,
          });

          this.questionDetails = response;
          this.errorMessage = '';
        },
        error: (error) => {
          this.errorMessage = error;
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }
}
