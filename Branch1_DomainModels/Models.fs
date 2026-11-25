namespace StudentGradesApp

open System

/// Application authorization role.
/// Teacher maps to Admin; Student maps to Viewer.
type Role =
    | Admin
    | Viewer

/// Single quiz/exam entry belonging to a student.
type ExamRecord =
    { QuizDate: string
      QuizTitle: string
      Grade: float
      QuizStatus: bool }

/// In-memory student record containing profile details and exam history.
type StudentRecord =
    { Id: Guid
      Name: string
      Email: string
      Grade: int
      Exams: ExamRecord list }

/// Account information captured during registration.
type RegisteredUser =
    { Name: string
      Email: string
      Password: string
      Role: Role
      Grade: int option }

/// Constant mock exam submission payload for teacher grading.
type MockSubmissionData =
    { StudentName: string
      StudentEmail: string
      Grade: int
      QuizTitle: string
      QuizDate: string
      EssayText: string }

/// Overall student standing derived from pass rate across exams.
type StudentStatus =
    | Good
    | Failing

/// Top-level navigation screen.
type Screen =
    | Registration
    | Login
    | Home

module Domain =

    let passingGradeThreshold = 50.0

    let roleFromLabel (label: string) : Role =
        match label.Trim().ToLowerInvariant() with
        | "teacher"
        | "admin" -> Admin
        | _ -> Viewer

    let roleDisplayName (role: Role) : string =
        match role with
        | Admin -> "Teacher (Admin)"
        | Viewer -> "Student (Viewer)"

    let roleShortName (role: Role) : string =
        match role with
        | Admin -> "Admin"
        | Viewer -> "Viewer"

    let isAdmin (role: Role) : bool =
        match role with
        | Admin -> true
        | Viewer -> false

    let isViewer (role: Role) : bool =
        not (isAdmin role)

    /// Computes average grade across a student's exams (0.0 when no exams exist).
    let calculateAverage (exams: ExamRecord list) : float =
        match exams with
        | [] -> 0.0
        | xs -> xs |> List.averageBy (fun exam -> exam.Grade)

    /// Computes percentage (0..100) of passed exams.
    let passPercentage (exams: ExamRecord list) : float =
        match exams with
        | [] -> 0.0
        | xs ->
            let passedCount = xs |> List.filter (fun exam -> exam.QuizStatus) |> List.length
            (float passedCount / float xs.Length) * 100.0

    /// Returns Good only if strictly more than 50% of exams have QuizStatus = true; otherwise Failing.
    let calculateStatistics (exams: ExamRecord list) : StudentStatus =
        if passPercentage exams > 50.0 then Good else Failing

    let statusText (status: StudentStatus) : string =
        match status with
        | Good -> "Good"
        | Failing -> "Failing"

    let examStatusText (passed: bool) : string =
        if passed then "Pass" else "Fail"

    let isValidEmail (email: string) : bool =
        let trimmed = email.Trim()
        let atIdx = trimmed.IndexOf('@')
        let dotIdx = trimmed.LastIndexOf('.')
        not (String.IsNullOrWhiteSpace trimmed)
        && atIdx > 0
        && dotIdx > atIdx + 1
        && dotIdx < trimmed.Length - 1
        && not (trimmed.Contains(" "))

    let isValidGrade (grade: int) : bool =
        grade >= 1 && grade <= 12

    let isValidScore (score: float) : bool =
        not (Double.IsNaN score) && not (Double.IsInfinity score) && score >= 0.0 && score <= 100.0

    /// Generates a varied set of dummy exams for newly created students.
    let generateDummyExams (seedText: string) : ExamRecord list =
        let hash =
            seedText.Trim().ToLowerInvariant()
            |> Seq.fold (fun acc ch -> (acc * 31 + int ch) &&& 0x7FFFFFFF) 17

        let score1 = float (45 + (hash % 51))
        let score2 = float (35 + ((hash / 7) % 61))
        let score3 = float (50 + ((hash / 13) % 48))

        [ { QuizDate = "05/10/2026"
            QuizTitle = "Quiz 1: Foundations"
            Grade = score1
            QuizStatus = score1 >= passingGradeThreshold }
          { QuizDate = "19/10/2026"
            QuizTitle = "Quiz 2: Applied Practice"
            Grade = score2
            QuizStatus = score2 >= passingGradeThreshold }
          { QuizDate = "02/11/2026"
            QuizTitle = "Midterm Evaluation"
            Grade = score3
            QuizStatus = score3 >= passingGradeThreshold } ]

    /// Finds the students with the highest and lowest exam averages.
    let highestAndLowestAverageStudents
        (students: StudentRecord list)
        : (StudentRecord * float) option * (StudentRecord * float) option =
        let candidates =
            let withExams = students |> List.filter (fun s -> not s.Exams.IsEmpty)
            if not withExams.IsEmpty then withExams else students

        match candidates with
        | [] -> None, None
        | xs ->
            let withAverages = xs |> List.map (fun s -> s, calculateAverage s.Exams)
            let highest = withAverages |> List.maxBy snd
            let lowest = withAverages |> List.minBy snd
            Some highest, Some lowest

    /// Seeded student records stored purely in memory.
    let seededStudents: StudentRecord list =
        [ { Id = Guid.Parse("11111111-1111-1111-1111-111111111111")
            Name = "Ali"
            Email = "one@gmail.com"
            Grade = 11
            Exams =
              [ { QuizDate = "25/7/2026"
                  QuizTitle = "chapter 1 quiz"
                  Grade = 25.0
                  QuizStatus = false } ] }
          { Id = Guid.Parse("22222222-2222-2222-2222-222222222222")
            Name = "Layla Hassan"
            Email = "layla.hassan@school.edu"
            Grade = 12
            Exams =
              [ { QuizDate = "10/8/2026"
                  QuizTitle = "Algorithms Basics"
                  Grade = 88.0
                  QuizStatus = true }
                { QuizDate = "18/9/2026"
                  QuizTitle = "Data Structures Quiz"
                  Grade = 92.0
                  QuizStatus = true } ] }
          { Id = Guid.Parse("33333333-3333-3333-3333-333333333333")
            Name = "Omar Khaled"
            Email = "omar.khaled@school.edu"
            Grade = 10
            Exams =
              [ { QuizDate = "02/9/2026"
                  QuizTitle = "Functions & Recursion"
                  Grade = 75.0
                  QuizStatus = true }
                { QuizDate = "15/9/2026"
                  QuizTitle = "Pattern Matching"
                  Grade = 60.0
                  QuizStatus = true }
                { QuizDate = "01/10/2026"
                  QuizTitle = "Higher-Order Functions"
                  Grade = 40.0
                  QuizStatus = false } ] }
          { Id = Guid.Parse("44444444-4444-4444-4444-444444444444")
            Name = "Nour Tarek"
            Email = "nour.tarek@school.edu"
            Grade = 9
            Exams =
              [ { QuizDate = "12/9/2026"
                  QuizTitle = "Intro to Types"
                  Grade = 55.0
                  QuizStatus = true }
                { QuizDate = "28/9/2026"
                  QuizTitle = "Lists and Records"
                  Grade = 35.0
                  QuizStatus = false } ] } ]

    /// Constant mock student submission displayed to teachers for grading.
    let constantMockSubmission: MockSubmissionData =
        { StudentName = "Sara Mahmoud"
          StudentEmail = "sara.mahmoud@school.edu"
          Grade = 11
          QuizTitle = "Chapter 2 Essay: Immutable Data Modeling"
          QuizDate = "25/11/2026"
          EssayText =
            "Immutable records and discriminated unions make student grade tracking predictable and safer to maintain. "
            + "Instead of mutating shared tables directly, every quiz submission or profile edit produces a clean updated snapshot "
            + "of the student list. This eliminates accidental side effects when calculating averages and pass/fail statistics, "
            + "and makes role-based access control between Admin (Teacher) and Viewer (Student) straightforward to reason about." }
