namespace StudentGradesApp.Views

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open StudentGradesApp

module StudentsTableView =

    let private columnDefs = "1.6*, 0.7*, 2.1*, 1.0*, 1.1*, 0.9*"

    let private headerCell (col: int) (text: string) : IView =
        TextBlock.create [
            Grid.column col
            TextBlock.text text
            TextBlock.fontSize 12.0
            TextBlock.fontWeight FontWeight.Bold
            TextBlock.foreground Theme.textMuted
            TextBlock.verticalAlignment VerticalAlignment.Center
        ] :> IView

    let private studentRow (appState: IWritable<AppState>) (isAdmin: bool) (student: StudentRecord) : IView =
        let avgText =
            if student.Exams.IsEmpty then "0.0"
            else sprintf "%.1f" (Domain.calculateAverage student.Exams)

        let stats = Domain.calculateStatistics student.Exams
        let statsLabel = Domain.statusText stats
        let statsBg = if stats = Good then Theme.successBg else Theme.dangerBg
        let statsFg = if stats = Good then Theme.successText else Theme.dangerText

        let isMenuOpen = isAdmin && appState.Current.OpenMenuStudentId = Some student.Id

        Border.create [
            Border.background Theme.bgInput
            Border.borderBrush Theme.borderSubtle
            Border.borderThickness 1.0
            Border.cornerRadius 10.0
            Border.padding (14.0, 10.0)
            Border.child (
                StackPanel.create [
                    StackPanel.spacing 8.0
                    StackPanel.children [
                        Grid.create [
                            Grid.columnDefinitions columnDefs
                            Grid.columnSpacing 10.0
                            Grid.children [
                                TextBlock.create [
                                    Grid.column 0
                                    TextBlock.text student.Name
                                    TextBlock.fontSize 14.0
                                    TextBlock.fontWeight FontWeight.SemiBold
                                    TextBlock.foreground Theme.textPrimary
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                    TextBlock.textWrapping TextWrapping.Wrap
                                ]
                                TextBlock.create [
                                    Grid.column 1
                                    TextBlock.text (sprintf "Grade %d" student.Grade)
                                    TextBlock.fontSize 13.0
                                    TextBlock.foreground Theme.textSecondary
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                TextBlock.create [
                                    Grid.column 2
                                    TextBlock.text student.Email
                                    TextBlock.fontSize 13.0
                                    TextBlock.foreground Theme.textSecondary
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                    TextBlock.textWrapping TextWrapping.Wrap
                                ]
                                TextBlock.create [
                                    Grid.column 3
                                    TextBlock.text avgText
                                    TextBlock.fontSize 14.0
                                    TextBlock.fontWeight FontWeight.Bold
                                    TextBlock.foreground Theme.textPrimary
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                Border.create [
                                    Grid.column 4
                                    Border.verticalAlignment VerticalAlignment.Center
                                    Border.child (Theme.badge statsLabel statsBg statsFg)
                                ]
                                Button.create [
                                    Grid.column 5
                                    Button.content "⋯"
                                    Button.fontSize 16.0
                                    Button.fontWeight FontWeight.Bold
                                    Button.padding (12.0, 4.0)
                                    Button.cornerRadius 6.0
                                    Button.background Theme.bgElevated
                                    Button.foreground Theme.textPrimary
                                    Button.borderBrush Theme.borderSubtle
                                    Button.borderThickness 1.0
                                    Button.horizontalAlignment HorizontalAlignment.Right
                                    Button.isEnabled isAdmin
                                    Button.onClick (fun _ ->
                                        let nextOpen =
                                            if appState.Current.OpenMenuStudentId = Some student.Id then None
                                            else Some student.Id
                                        appState.Set { appState.Current with OpenMenuStudentId = nextOpen })
                                ]
                            ]
                        ]

                        if isMenuOpen then
                            Border.create [
                                Border.background Theme.bgElevated
                                Border.borderBrush Theme.borderAccent
                                Border.borderThickness 1.0
                                Border.cornerRadius 8.0
                                Border.padding (12.0, 8.0)
                                Border.child (
                                    DockPanel.create [
                                        DockPanel.children [
                                            TextBlock.create [
                                                DockPanel.dock Dock.Left
                                                TextBlock.text (sprintf "Actions for %s" student.Name)
                                                TextBlock.fontSize 12.0
                                                TextBlock.foreground Theme.textSecondary
                                                TextBlock.verticalAlignment VerticalAlignment.Center
                                            ]
                                            StackPanel.create [
                                                DockPanel.dock Dock.Right
                                                StackPanel.orientation Orientation.Horizontal
                                                StackPanel.horizontalAlignment HorizontalAlignment.Right
                                                StackPanel.spacing 8.0
                                                StackPanel.children [
                                                    Button.create [
                                                        Button.content "Edit Student Info"
                                                        Button.isEnabled isAdmin
                                                        Button.padding (12.0, 6.0)
                                                        Button.cornerRadius 6.0
                                                        Button.background Theme.accentPrimary
                                                        Button.foreground "#FFFFFF"
                                                        Button.fontSize 12.0
                                                        Button.fontWeight FontWeight.SemiBold
                                                        Button.onClick (fun _ ->
                                                            appState.Set
                                                                { appState.Current with
                                                                    ActiveModal = EditStudentModal student
                                                                    OpenMenuStudentId = None })
                                                    ]
                                                    Button.create [
                                                        Button.content "Delete Student Info"
                                                        Button.isEnabled isAdmin
                                                        Button.padding (12.0, 6.0)
                                                        Button.cornerRadius 6.0
                                                        Button.background Theme.dangerButtonBg
                                                        Button.foreground "#FFFFFF"
                                                        Button.fontSize 12.0
                                                        Button.fontWeight FontWeight.SemiBold
                                                        Button.onClick (fun _ ->
                                                            match State.deleteStudent student.Id appState.Current with
                                                            | Ok nextState -> appState.Set nextState
                                                            | Error _ -> ())
                                                    ]
                                                ]
                                            ]
                                        ]
                                    ]
                                )
                            ] :> IView
                    ]
                ]
            )
        ] :> IView

    let view (appState: IWritable<AppState>) (isAdmin: bool) : IView =
        let students = appState.Current.Students

        Border.create [
            Border.background Theme.bgCard
            Border.borderBrush Theme.borderSubtle
            Border.borderThickness 1.0
            Border.cornerRadius 14.0
            Border.padding 22.0
            Border.child (
                StackPanel.create [
                    StackPanel.spacing 14.0
                    StackPanel.children [
                        DockPanel.create [
                            DockPanel.children [
                                StackPanel.create [
                                    DockPanel.dock Dock.Left
                                    StackPanel.spacing 4.0
                                    StackPanel.children [
                                        StackPanel.create [
                                            StackPanel.orientation Orientation.Horizontal
                                            StackPanel.spacing 10.0
                                            StackPanel.children [
                                                TextBlock.create [
                                                    TextBlock.text "Students Directory"
                                                    TextBlock.fontSize 18.0
                                                    TextBlock.fontWeight FontWeight.Bold
                                                    TextBlock.foreground Theme.textPrimary
                                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                                ]
                                                Theme.badge (sprintf "%d students" students.Length) Theme.accentSoft "#C7D2FE"
                                            ]
                                        ]
                                        TextBlock.create [
                                            TextBlock.text (
                                                if isAdmin then
                                                    "Admin mode: Add, edit, delete, or grade mock student submissions."
                                                else
                                                    "Viewer mode: Read-only view (CRUD controls are disabled)."
                                            )
                                            TextBlock.fontSize 12.0
                                            TextBlock.foreground Theme.textSecondary
                                        ]
                                    ]
                                ]

                                StackPanel.create [
                                    DockPanel.dock Dock.Right
                                    StackPanel.orientation Orientation.Horizontal
                                    StackPanel.horizontalAlignment HorizontalAlignment.Right
                                    StackPanel.verticalAlignment VerticalAlignment.Center
                                    StackPanel.spacing 10.0
                                    StackPanel.children [
                                        Button.create [
                                            Button.content "Mock student submission"
                                            Button.isEnabled isAdmin
                                            Button.padding (14.0, 9.0)
                                            Button.cornerRadius 8.0
                                            Button.background Theme.bgElevated
                                            Button.foreground Theme.textPrimary
                                            Button.borderBrush Theme.borderSubtle
                                            Button.borderThickness 1.0
                                            Button.fontSize 13.0
                                            Button.fontWeight FontWeight.SemiBold
                                            Button.onClick (fun _ ->
                                                appState.Set
                                                    { appState.Current with
                                                        ActiveModal = MockSubmissionModal
                                                        OpenMenuStudentId = None })
                                        ]
                                        Button.create [
                                            Button.content "+ Add Student"
                                            Button.isEnabled isAdmin
                                            Button.padding (14.0, 9.0)
                                            Button.cornerRadius 8.0
                                            Button.background Theme.accentPrimary
                                            Button.foreground "#FFFFFF"
                                            Button.fontSize 13.0
                                            Button.fontWeight FontWeight.SemiBold
                                            Button.onClick (fun _ ->
                                                appState.Set
                                                    { appState.Current with
                                                        ActiveModal = AddStudentModal
                                                        OpenMenuStudentId = None })
                                        ]
                                    ]
                                ]
                            ]
                        ]

                        Border.create [
                            Border.padding (14.0, 6.0)
                            Border.child (
                                Grid.create [
                                    Grid.columnDefinitions columnDefs
                                    Grid.columnSpacing 10.0
                                    Grid.children [
                                        headerCell 0 "STUDENT NAME"
                                        headerCell 1 "GRADE"
                                        headerCell 2 "EMAIL"
                                        headerCell 3 "AVERAGE"
                                        headerCell 4 "STATISTICS"
                                        headerCell 5 "ACTIONS"
                                    ]
                                ]
                            )
                        ]

                        if students.IsEmpty then
                            Border.create [
                                Border.background Theme.bgInput
                                Border.borderBrush Theme.borderSubtle
                                Border.borderThickness 1.0
                                Border.cornerRadius 8.0
                                Border.padding 18.0
                                Border.child (
                                    TextBlock.create [
                                        TextBlock.text "No students in the directory yet."
                                        TextBlock.fontSize 13.0
                                        TextBlock.foreground Theme.textSecondary
                                    ]
                                )
                            ] :> IView
                        else
                            StackPanel.create [
                                StackPanel.spacing 8.0
                                StackPanel.children (students |> List.map (studentRow appState isAdmin))
                            ] :> IView
                    ]
                ]
            )
        ] :> IView
