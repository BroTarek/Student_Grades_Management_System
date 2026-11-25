namespace StudentGradesApp

open System

type ModalState =
    | NoModal
    | AddStudentModal
    | EditStudentModal of StudentRecord
    | MockSubmissionModal

type AppState =
    { CurrentScreen: Screen
      RegisteredUser: RegisteredUser option
      LoggedInUser: RegisteredUser option
      Students: StudentRecord list
      ActiveModal: ModalState
      OpenMenuStudentId: Guid option }

module State =

    let initialState: AppState =
        { CurrentScreen = Registration
          RegisteredUser = None
          LoggedInUser = None
          Students = Domain.seededStudents
          ActiveModal = NoModal
          OpenMenuStudentId = None }

    let private requireAdmin (state: AppState) : Result<unit, string> =
        match state.LoggedInUser with
        | Some user when Domain.isAdmin user.Role -> Ok ()
        | _ -> Error "Only Teacher (Admin) accounts can modify student records."

    let findStudentForUser (user: RegisteredUser) (students: StudentRecord list) : StudentRecord option =
        let targetEmail = user.Email.Trim().ToLowerInvariant()
        students
        |> List.tryFind (fun s -> s.Email.Trim().ToLowerInvariant() = targetEmail)

    let registerUser
        (name: string)
        (role: Role)
        (email: string)
        (password: string)
        (grade: int option)
        (state: AppState)
        : Result<AppState, string> =
        let trimmedName = name.Trim()
        let trimmedEmail = email.Trim()

        if String.IsNullOrWhiteSpace trimmedName then
            Error "Please enter your name."
        elif not (Domain.isValidEmail trimmedEmail) then
            Error "Please enter a valid email address."
        elif String.IsNullOrWhiteSpace password then
            Error "Please enter a password."
        else
            match role with
            | Viewer ->
                match grade with
                | Some g when Domain.isValidGrade g ->
                    let user =
                        { Name = trimmedName
                          Email = trimmedEmail
                          Password = password
                          Role = Viewer
                          Grade = Some g }

                    let existing = findStudentForUser user state.Students
                    let updatedStudents =
                        match existing with
                        | Some _ -> state.Students
                        | None ->
                            state.Students
                            @ [ { Id = Guid.NewGuid()
                                  Name = trimmedName
                                  Email = trimmedEmail
                                  Grade = g
                                  Exams = Domain.generateDummyExams trimmedEmail } ]

                    Ok
                        { state with
                            RegisteredUser = Some user
                            Students = updatedStudents
                            CurrentScreen = Login
                            ActiveModal = NoModal
                            OpenMenuStudentId = None }
                | _ -> Error "Please select a grade between 1 and 12."
            | Admin ->
                let user =
                    { Name = trimmedName
                      Email = trimmedEmail
                      Password = password
                      Role = Admin
                      Grade = None }

                Ok
                    { state with
                        RegisteredUser = Some user
                        CurrentScreen = Login
                        ActiveModal = NoModal
                        OpenMenuStudentId = None }

    let loginUser (email: string) (password: string) (state: AppState) : Result<AppState, string> =
        let trimmedEmail = email.Trim().ToLowerInvariant()

        match state.RegisteredUser with
        | None -> Error "No registered account found. Please register first."
        | Some registered ->
            if registered.Email.Trim().ToLowerInvariant() = trimmedEmail
               && registered.Password = password then
                Ok
                    { state with
                        LoggedInUser = Some registered
                        CurrentScreen = Home
                        ActiveModal = NoModal
                        OpenMenuStudentId = None }
            else
                Error "Invalid email or password. Please check your credentials."

    let logout (state: AppState) : AppState =
        { state with
            LoggedInUser = None
            CurrentScreen = Login
            ActiveModal = NoModal
            OpenMenuStudentId = None }

    let goToRegistration (state: AppState) : AppState =
        { state with
            LoggedInUser = None
            CurrentScreen = Registration
            ActiveModal = NoModal
            OpenMenuStudentId = None }

    let addStudent (name: string) (email: string) (grade: int) (state: AppState) : Result<AppState, string> =
        match requireAdmin state with
        | Error msg -> Error msg
        | Ok () ->
            let trimmedName = name.Trim()
            let trimmedEmail = email.Trim()

            if String.IsNullOrWhiteSpace trimmedName then
                Error "Student name is required."
            elif not (Domain.isValidEmail trimmedEmail) then
                Error "Please enter a valid student email."
            elif not (Domain.isValidGrade grade) then
                Error "Grade must be between 1 and 12."
            elif state.Students
                 |> List.exists (fun s -> s.Email.Trim().ToLowerInvariant() = trimmedEmail.ToLowerInvariant()) then
                Error "A student with this email already exists."
            else
                let newStudent =
                    { Id = Guid.NewGuid()
                      Name = trimmedName
                      Email = trimmedEmail
                      Grade = grade
                      Exams = Domain.generateDummyExams trimmedEmail }

                Ok
                    { state with
                        Students = state.Students @ [ newStudent ]
                        ActiveModal = NoModal
                        OpenMenuStudentId = None }

    let updateStudent
        (studentId: Guid)
        (name: string)
        (email: string)
        (grade: int)
        (state: AppState)
        : Result<AppState, string> =
        match requireAdmin state with
        | Error msg -> Error msg
        | Ok () ->
            let trimmedName = name.Trim()
            let trimmedEmail = email.Trim()

            if String.IsNullOrWhiteSpace trimmedName then
                Error "Student name is required."
            elif not (Domain.isValidEmail trimmedEmail) then
                Error "Please enter a valid student email."
            elif not (Domain.isValidGrade grade) then
                Error "Grade must be between 1 and 12."
            elif state.Students
                 |> List.exists (fun s ->
                     s.Id <> studentId
                     && s.Email.Trim().ToLowerInvariant() = trimmedEmail.ToLowerInvariant()) then
                Error "Another student already uses this email."
            else
                let updatedStudents =
                    state.Students
                    |> List.map (fun s ->
                        if s.Id = studentId then
                            { s with
                                Name = trimmedName
                                Email = trimmedEmail
                                Grade = grade }
                        else
                            s)

                Ok
                    { state with
                        Students = updatedStudents
                        ActiveModal = NoModal
                        OpenMenuStudentId = None }

    let deleteStudent (studentId: Guid) (state: AppState) : Result<AppState, string> =
        match requireAdmin state with
        | Error msg -> Error msg
        | Ok () ->
            Ok
                { state with
                    Students = state.Students |> List.filter (fun s -> s.Id <> studentId)
                    ActiveModal = NoModal
                    OpenMenuStudentId = None }

    let submitMockGrade (score: float) (state: AppState) : Result<AppState, string> =
        match requireAdmin state with
        | Error msg -> Error msg
        | Ok () ->
            if not (Domain.isValidScore score) then
                Error "Please enter a valid numeric grade between 0 and 100."
            else
                let submission = Domain.constantMockSubmission
                let newExam =
                    { QuizDate = submission.QuizDate
                      QuizTitle = submission.QuizTitle
                      Grade = score
                      QuizStatus = score >= Domain.passingGradeThreshold }

                let targetEmail = submission.StudentEmail.Trim().ToLowerInvariant()
                let exists =
                    state.Students
                    |> List.exists (fun s -> s.Email.Trim().ToLowerInvariant() = targetEmail)

                let updatedStudents =
                    if exists then
                        state.Students
                        |> List.map (fun s ->
                            if s.Email.Trim().ToLowerInvariant() = targetEmail then
                                { s with Exams = s.Exams @ [ newExam ] }
                            else
                                s)
                    else
                        state.Students
                        @ [ { Id = Guid.NewGuid()
                              Name = submission.StudentName
                              Email = submission.StudentEmail
                              Grade = submission.Grade
                              Exams = [ newExam ] } ]

                Ok
                    { state with
                        Students = updatedStudents
                        ActiveModal = NoModal
                        OpenMenuStudentId = None }
