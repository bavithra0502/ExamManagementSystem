import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ExamService, ExamSaveRequest } from '../../services/exam-service';
import { ExamDtlRow } from '../../models/exam-dtl-row';

@Component({
  selector: 'app-exam-entry',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './exam-entry.html',
  styleUrl: './exam-entry.scss',
})
export class ExamEntry implements OnInit {

  constructor(public examService: ExamService, private router: Router) { }

  //---- simple dropdown selections (store the selected ID only) ----
  selectedStudentId: number | null = null;
  selectedSubjectId: number | null = null;

  //---- form fields ----
  examYear: number | null = new Date().getFullYear();
  marks: number | null = null;

  //---- rows added to the table, ready to be saved ----
  examRows: ExamDtlRow[] = [];

  //---- ui state ----
  errorMessage = '';
  saving = false;

  ngOnInit(): void {
    this.examService.getAllStudents();
    this.examService.getAllSubjects();
    //needed so we know which students already have a record for a given year
    this.examService.getAllExamResults();
  }

  //true if the given student already has a saved exam record for the currently entered year
  isStudentRegisteredForYear(studentId: number): boolean {
    if (!this.examYear) { return false; }
    return this.examService.examResults()
      .some(e => e.StudentID === studentId && e.ExamYear === Number(this.examYear));
  }

  //called when the Student dropdown changes - blocks the pick immediately if already registered
  onStudentChange(): void {
    this.errorMessage = '';
    if (this.selectedStudentId && this.isStudentRegisteredForYear(this.selectedStudentId)) {
      this.errorMessage = `This student already has an exam record for ${this.examYear}. Please choose a different student or year.`;
      this.selectedStudentId = null;
    }
  }

  //called when Exam Year changes - re-checks the currently selected student against the new year
  onYearChange(): void {
    this.errorMessage = '';
    if (this.selectedStudentId && this.isStudentRegisteredForYear(this.selectedStudentId)) {
      this.errorMessage = `This student already has an exam record for ${this.examYear}. Please choose a different student or year.`;
      this.selectedStudentId = null;
    }
  }

  //formats a StudentID as a 4-digit code, e.g. 1 -> "0001"
  formatStudentId(id: number): string {
    return id.toString().padStart(4, '0');
  }

  //subjects still available to pick (already-added ones are removed from the dropdown)
  get availableSubjects() {
    return this.examService.subjects().filter(sub => !this.examRows.some(r => r.SubjectID === sub.SubjectID));
  }

  get totalMarks(): number {
    return this.examRows.reduce((sum, r) => sum + (Number(r.Marks) || 0), 0);
  }

  //Save is only enabled once a student, year, and EVERY subject has been added
  get canSave(): boolean {
    const totalSubjects = this.examService.subjects().length;
    return !!this.selectedStudentId
      && !!this.examYear
      && totalSubjects > 0
      && this.examRows.length === totalSubjects
      && !this.isStudentRegisteredForYear(this.selectedStudentId)
      && !this.saving;
  }

  //how many subjects are still missing (used to show a helpful message)
  get remainingSubjectsCount(): number {
    return this.examService.subjects().length - this.examRows.length;
  }

  //true if the currently selected student turns out to already be registered for the year
  //(safety net in case exam results finish loading slightly after the student was picked)
  get isSelectedStudentBlocked(): boolean {
    return !!this.selectedStudentId && this.isStudentRegisteredForYear(this.selectedStudentId);
  }

  addSubjectRow(): void {
    this.errorMessage = '';

    if (!this.selectedSubjectId) {
      this.errorMessage = 'Please select a subject.';
      return;
    }

    if (this.marks === null || this.marks === undefined || this.marks < 0 || this.marks > 100) {
      this.errorMessage = 'Marks must be between 0 and 100.';
      return;
    }

    const subject = this.examService.subjects().find(s => s.SubjectID === this.selectedSubjectId);
    if (!subject) {
      this.errorMessage = 'Please select a valid subject.';
      return;
    }

    this.examRows.push({
      SubjectID: subject.SubjectID,
      SubjectName: subject.SubjectName,
      Marks: this.marks
    });

    //reset subject/marks inputs so the next subject can be added
    this.selectedSubjectId = null;
    this.marks = null;
  }

  removeRow(index: number): void {
    this.examRows.splice(index, 1);
  }

  onSave(): void {
    this.errorMessage = '';

    if (!this.canSave) {
      this.errorMessage = 'Please select a student, enter a year, and add marks for every subject before saving.';
      return;
    }

    const request: ExamSaveRequest = {
      StudentID: this.selectedStudentId!,
      ExamYear: this.examYear!,
      Subjects: this.examRows.map(r => ({ SubjectID: r.SubjectID, Marks: r.Marks }))
    };

    this.saving = true;
    this.examService.saveExam(request).subscribe({
      next: () => {
        this.saving = false;
        //go straight to the list page to show the saved record
        this.router.navigate(['/exam-list']);
      },
      error: (err) => {
        this.saving = false;
        this.errorMessage = err?.error?.message || 'Something went wrong while saving. Please try again.';
      }
    });
  }
}
