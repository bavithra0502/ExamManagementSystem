import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ExamService } from '../../services/exam-service';
import { ExamDtlResult } from '../../models/exam-result';

@Component({
  selector: 'app-exam-edit',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './exam-edit.html',
  styleUrl: './exam-edit.scss',
})
export class ExamEdit implements OnInit {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    public examService: ExamService
  ) { }

  masterId!: number;
  studentName = '';
  examYear = 0;
  subjectRows: ExamDtlResult[] = [];

  loading = true;
  saving = false;
  errorMessage = '';

  ngOnInit(): void {
    this.masterId = Number(this.route.snapshot.paramMap.get('id'));

    this.examService.getExamById(this.masterId).subscribe({
      next: (exam) => {
        this.studentName = exam.StudentName;
        this.examYear = exam.ExamYear;
        //copy the array so editing here doesn't touch the original list data
        this.subjectRows = exam.Subjects.map(s => ({ ...s }));
        this.loading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load this exam record.';
        this.loading = false;
      }
    });
  }

  get totalMarks(): number {
    return this.subjectRows.reduce((sum, r) => sum + (Number(r.Marks) || 0), 0);
  }

  onSave(): void {
    this.errorMessage = '';

    for (const row of this.subjectRows) {
      if (row.Marks === null || row.Marks === undefined || row.Marks < 0 || row.Marks > 100) {
        this.errorMessage = `Marks for ${row.SubjectName} must be between 0 and 100.`;
        return;
      }
    }

    this.saving = true;
    const request = {
      Subjects: this.subjectRows.map(r => ({ SubjectID: r.SubjectID, Marks: r.Marks }))
    };

    this.examService.updateExam(this.masterId, request).subscribe({
      next: () => {
        this.saving = false;
        this.router.navigate(['/exam-list']);
      },
      error: (err) => {
        this.saving = false;
        this.errorMessage = err?.error?.message || 'Something went wrong while saving. Please try again.';
      }
    });
  }
}
