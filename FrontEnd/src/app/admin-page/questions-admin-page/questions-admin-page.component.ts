import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { LinkButtonComponent } from '../../global-components/link-button/link-button.component';
import { QuestionsAdminService } from '../../services/admin-services/questionsadmin.service';
import { QuestionLight } from '../../models/admin-models/questionlight.model';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';

@Component({
  selector: 'app-questions-admin-page',
  standalone: true,
  imports: [LinkButtonComponent],
  templateUrl: './questions-admin-page.component.html',
  styleUrl: './questions-admin-page.component.css',
})
export class QuestionsAdminPageComponent implements OnInit {
  private destroyRef = inject(DestroyRef);
  private router = inject(Router);
  questionsAdminService = inject(QuestionsAdminService);
  questionList?: QuestionLight[];

  ngOnInit(): void {
    const subscription = this.questionsAdminService
      .getAllQuestions()
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error));
        })
      )
      .subscribe({
        next: (response: QuestionLight[]) => {
          this.questionList = response;
        },
        error: (error) => {
          console.log(error);
        },
      });
    this.destroyRef.onDestroy(() => subscription.unsubscribe());
  }

  selectQuestion(questionId: number) {
    this.router.navigate(['admin/questions/details'], {
      queryParams: { questionId: questionId },
    });
  }
}
