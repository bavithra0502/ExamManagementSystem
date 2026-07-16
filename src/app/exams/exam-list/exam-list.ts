import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ExamService } from '../../services/exam-service';

@Component({
  selector: 'app-exam-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './exam-list.html',
  styleUrl: './exam-list.scss',
})
export class ExamList implements OnInit {

  constructor(public examService: ExamService) { }

  ngOnInit(): void {
    this.examService.getAllExamResults();
  }
}
