namespace StudentGradesApp

open System

module Tests =

    let private assertTrue (label: string) (condition: bool) =
        if not condition then
            failwithf "Assertion failed: %s" label

    let private assertEqual (label: string) (expected: 'T) (actual: 'T) =
        if expected <> actual then
            failwithf "Assertion failed (%s): expected %A, got %A" label expected actual

    let runAll () =
        // 1. Seeded Ali record matches specification
        let aliOpt =
            Domain.seededStudents
            |> List.tryFind (fun s -> s.Email = "one@gmail.com")

        assertTrue "Seeded student Ali exists" (Option.isSome aliOpt)
        let ali = aliOpt.Value
        assertEqual "Ali name" "Ali" ali.Name
        assertEqual "Ali grade" 11 ali.Grade
        assertEqual "Ali exam count" 1 ali.Exams.Length
        assertEqual "Ali exam date" "25/7/2026" ali.Exams.Head.QuizDate
        assertEqual "Ali exam title" "chapter 1 quiz" ali.Exams.Head.QuizTitle
        assertEqual "Ali exam grade" 25.0 ali.Exams.Head.Grade
        assertEqual "Ali exam status" false ali.Exams.Head.QuizStatus
        assertEqual "Ali average" 25.0 (Domain.calculateAverage ali.Exams)
        assertEqual "Ali statistics" Failing (Domain.calculateStatistics ali.Exams)

        // 2. Statistics boundary (> 50% true -> Good; <= 50% -> Failing)
        let halfPass =
            [ { QuizDate = "1/1/2026"; QuizTitle = "Q1"; Grade = 80.0; QuizStatus = true }
              { QuizDate = "2/1/2026"; QuizTitle = "Q2"; Grade = 30.0; QuizStatus = false } ]
        assertEqual "Exactly 50% pass is Failing" Failing (Domain.calculateStatistics halfPass)

        let overHalfPass =
            halfPass
            @ [ { QuizDate = "3/1/2026"; QuizTitle = "Q3"; Grade = 70.0; QuizStatus = true } ]
        assertEqual "66.7% pass is Good" Good (Domain.calculateStatistics overHalfPass)

        // 3. Role mapping: Teacher -> Admin, Student -> Viewer
        assertEqual "Teacher maps to Admin" Admin (Domain.roleFromLabel "Teacher")
        assertEqual "Student maps to Viewer" Viewer (Domain.roleFromLabel "Student")

        // 4. Registration & Login flow for Student (Viewer)
        let s0 = State.initialState
        let invalidGradeRes = State.registerUser "Ali" Viewer "one@gmail.com" "secret" (Some 13) s0
        assertTrue "Grade 13 rejected" (Result.isError invalidGradeRes)

        let regStudentRes = State.registerUser "Ali" Viewer "one@gmail.com" "secret" (Some 11) s0
        assertTrue "Student registration succeeds" (Result.isOk regStudentRes)
        let s1 = Result.defaultWith (fun e -> failwith e) regStudentRes
        assertEqual "Transitions to Login after registration" Login s1.CurrentScreen

        let badLogin = State.loginUser "one@gmail.com" "wrong" s1
        assertTrue "Wrong password rejected" (Result.isError badLogin)

        let goodStudentLogin = State.loginUser "ONE@GMAIL.COM" "secret" s1
        assertTrue "Matching email/password logs in" (Result.isOk goodStudentLogin)
        let sStudentHome = Result.defaultWith (fun e -> failwith e) goodStudentLogin
        assertEqual "Logged in user is Viewer" (Some Viewer) (sStudentHome.LoggedInUser |> Option.map (fun u -> u.Role))
        assertEqual "Navigates to Home" Home sStudentHome.CurrentScreen

        // 5. Viewer cannot execute CRUD mutations
        assertTrue "Viewer cannot add student" (Result.isError (State.addStudent "Test" "t@test.com" 5 sStudentHome))
        assertTrue "Viewer cannot delete student" (Result.isError (State.deleteStudent ali.Id sStudentHome))
        assertTrue "Viewer cannot submit mock grade" (Result.isError (State.submitMockGrade 80.0 sStudentHome))

        // 6. Registration & Login flow for Teacher (Admin) + CRUD + Mock submission
        let regTeacherRes = State.registerUser "Ms. Huda" Admin "huda@school.edu" "admin123" None s0
        assertTrue "Teacher registration succeeds" (Result.isOk regTeacherRes)
        let sTeacherReg = Result.defaultWith (fun e -> failwith e) regTeacherRes

        let loginTeacherRes = State.loginUser "huda@school.edu" "admin123" sTeacherReg
        assertTrue "Teacher login succeeds" (Result.isOk loginTeacherRes)
        let sAdminHome = Result.defaultWith (fun e -> failwith e) loginTeacherRes
        assertEqual "Logged in user is Admin" (Some Admin) (sAdminHome.LoggedInUser |> Option.map (fun u -> u.Role))

        // Add student
        let addedRes = State.addStudent "Zaid Rami" "zaid@school.edu" 8 sAdminHome
        assertTrue "Admin can add student" (Result.isOk addedRes)
        let sAdded = Result.defaultWith (fun e -> failwith e) addedRes
        assertEqual "Student count increased" (sAdminHome.Students.Length + 1) sAdded.Students.Length

        // Edit student
        let zaid = sAdded.Students |> List.find (fun s -> s.Email = "zaid@school.edu")
        let editedRes = State.updateStudent zaid.Id "Zaid Updated" "zaid.updated@school.edu" 9 sAdded
        assertTrue "Admin can edit student" (Result.isOk editedRes)
        let sEdited = Result.defaultWith (fun e -> failwith e) editedRes
        let zaidAfter = sEdited.Students |> List.find (fun s -> s.Id = zaid.Id)
        assertEqual "Edited name persisted" "Zaid Updated" zaidAfter.Name
        assertEqual "Edited grade persisted" 9 zaidAfter.Grade

        // Mock submission adds graded student entry
        let mockRes = State.submitMockGrade 88.0 sEdited
        assertTrue "Admin can submit mock grade" (Result.isOk mockRes)
        let sMocked = Result.defaultWith (fun e -> failwith e) mockRes
        let mockStudent =
            sMocked.Students
            |> List.find (fun s -> s.Email = Domain.constantMockSubmission.StudentEmail)
        assertEqual "Mock student exam recorded" 1 mockStudent.Exams.Length
        assertEqual "Mock student exam pass status" true mockStudent.Exams.Head.QuizStatus
        assertEqual "Mock student stats Good" Good (Domain.calculateStatistics mockStudent.Exams)

        // Delete student
        let deletedRes = State.deleteStudent zaid.Id sMocked
        assertTrue "Admin can delete student" (Result.isOk deletedRes)
        let sDeleted = Result.defaultWith (fun e -> failwith e) deletedRes
        assertTrue "Deleted student removed" (sDeleted.Students |> List.forall (fun s -> s.Id <> zaid.Id))

        printfn "All domain, auth, role-guard, and CRUD tests passed!"
        0
