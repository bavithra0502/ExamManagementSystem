export class ExamDtlResult {
    DtlsID: number = 0;
    SubjectID: number = 0;
    SubjectName: string = '';
    Marks: number = 0;
}

export class ExamResult {
    MasterID: number = 0;
    StudentID: number = 0;
    StudentName: string = '';
    Mail: string = '';
    ExamYear: number = 0;
    TotalMark: number = 0;
    PassOrFail: string = '';
    CreateTime: string = '';
    Subjects: ExamDtlResult[] = [];
}
