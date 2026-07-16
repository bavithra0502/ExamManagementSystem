import { Routes } from '@angular/router';
import { ExamEntry } from './exams/exam-entry/exam-entry';
import { ExamList } from './exams/exam-list/exam-list';
import { ExamEdit } from './exams/exam-edit/exam-edit';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'exam-entry',
        pathMatch: 'full'
    },
    {
        path: 'exam-entry',
        component: ExamEntry
    },
    {
        path: 'exam-list',
        component: ExamList
    },
    {
        path: 'exam-edit/:id',
        component: ExamEdit
    },
    {
        path: '**',
        redirectTo: 'exam-entry'
    }
];
