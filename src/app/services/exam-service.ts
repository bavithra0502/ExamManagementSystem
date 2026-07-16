import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, timeout } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Student } from '../models/student';
import { Subject } from '../models/subject';
import { ExamResult } from '../models/exam-result';

export interface ExamDtlRequest {
  SubjectID: number;
  Marks: number;
}

export interface ExamSaveRequest {
  StudentID: number;
  ExamYear: number;
  Subjects: ExamDtlRequest[];
}

@Injectable({
  providedIn: 'root',
})
export class ExamService {

  //signals holding the master lists used for the autofill fields
  students = signal<Student[]>([]);
  subjects = signal<Subject[]>([]);

  //signal holding every saved exam record (rendered in the results table)
  examResults = signal<ExamResult[]>([]);

  constructor(private httpClient: HttpClient) { }

  //1-Get all students (for the student autofill field)
  getAllStudents(): void {
    this.httpClient.get<Student[]>(environment.apiUrl + 'Students')
      .subscribe({
        next: response => this.students.set(response),
        error: error => console.log('Error loading students: ', error)
      });
  }

  //2-Get all subjects (for the subject autofill field)
  getAllSubjects(): void {
    this.httpClient.get<Subject[]>(environment.apiUrl + 'Subjects')
      .subscribe({
        next: response => this.subjects.set(response),
        error: error => console.log('Error loading subjects: ', error)
      });
  }

  //3-Get all saved exam results (for the results table)
  getAllExamResults(): void {
    this.httpClient.get<ExamResult[]>(environment.apiUrl + 'Exams')
      .subscribe({
        next: response => this.examResults.set(response),
        error: error => console.log('Error loading exam results: ', error)
      });
  }

  //4-Save a new exam record (ExamMaster + ExamDtls)
  //A 20s timeout stops the UI from spinning forever if the API never responds
  //(e.g. it crashed, or is unreachable).
  saveExam(request: ExamSaveRequest): Observable<ExamResult> {
    return this.httpClient.post<ExamResult>(environment.apiUrl + 'Exams', request)
      .pipe(
        timeout(20000),
        catchError(error => {
          if (error.name === 'TimeoutError') {
            return throwError(() => ({
              error: { message: 'The server took too long to respond. Please check that the API is running and try again.' }
            }));
          }
          return throwError(() => error);
        })
      );
  }

  //5-Get a single exam record by ID (used to pre-fill the edit page)
  getExamById(id: number): Observable<ExamResult> {
    return this.httpClient.get<ExamResult>(environment.apiUrl + 'Exams/' + id);
  }

  //6-Update the marks of an existing exam record
  updateExam(id: number, request: { Subjects: ExamDtlRequest[] }): Observable<ExamResult> {
    return this.httpClient.put<ExamResult>(environment.apiUrl + 'Exams/' + id, request);
  }
}
