namespace StudentGradesApp.Views

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open StudentGradesApp

module ExamsTableView =

    let private columnDefs = "1.2*, 2.4*, 1*, 1*"

    let private headerCell (col: int) (text: string) : IView =
        TextBlock.create [
            Grid.column col
            TextBlock.text text
            TextBlock.fontSize 12.0
            TextBlock.fontWeight FontWeight.Bold
            TextBlock.foreground Theme.textMuted
            TextBlock.verticalAlignment VerticalAlignment.Center
        ] :> IView

    let private examRow (exam: ExamRecord) : IView =
        let statusBg = if exam.QuizStatus then Theme.successBg else Theme.dangerBg
        let statusFg = if exam.QuizStatus then Theme.successText else Theme.dangerText

        Border.create [
            Border.background Theme.bgInput
            Border.borderBrush Theme.borderSubtle
            Border.borderThickness 1.0
            Border.cornerRadius 8.0
            Border.padding (14.0, 10.0)
            Border.child (
                Grid.create [
                    Grid.columnDefinitions columnDefs
                    Grid.columnSpacing 12.0
                    Grid.children [
                        TextBlock.create [
                            Grid.column 0
                            TextBlock.text exam.QuizDate
                            TextBlock.fontSize 13.0
                            TextBlock.foreground Theme.textSecondary
                            TextBlock.verticalAlignment VerticalAlignment.Center
                        ]
                        TextBlock.create [
                            Grid.column 1
                            TextBlock.text exam.QuizTitle
                            TextBlock.fontSize 14.0
                            TextBlock.fontWeight FontWeight.Medium
                            TextBlock.foreground Theme.textPrimary
                            TextBlock.verticalAlignment VerticalAlignment.Center
                            TextBlock.textWrapping TextWrapping.Wrap
                        ]
                        TextBlock.create [
                            Grid.column 2
                            TextBlock.text (sprintf "%.1f" exam.Grade)
                            TextBlock.fontSize 14.0
                            TextBlock.fontWeight FontWeight.Bold
                            TextBlock.foreground Theme.textPrimary
                            TextBlock.verticalAlignment VerticalAlignment.Center
                        ]
                        Border.create [
                            Grid.column 3
                            Border.verticalAlignment VerticalAlignment.Center
                            Border.child (Theme.badge (Domain.examStatusText exam.QuizStatus) statusBg statusFg)
                        ]
                    ]
                ]
            )
        ] :> IView

    let view (studentOpt: StudentRecord option) : IView =
        let exams =
            match studentOpt with
            | Some s -> s.Exams
            | None -> []

        Border.create [
            Border.background Theme.bgCard
            Border.borderBrush Theme.borderSubtle
            Border.borderThickness 1.0
            Border.cornerRadius 14.0
            Border.padding 22.0
            Border.child (
                StackPanel.create [
                    StackPanel.spacing 12.0
                    StackPanel.children [
                        StackPanel.create [
                            StackPanel.spacing 4.0
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.text "My Quiz & Exam Results"
                                    TextBlock.fontSize 18.0
                                    TextBlock.fontWeight FontWeight.Bold
                                    TextBlock.foreground Theme.textPrimary
                                ]
                                TextBlock.create [
                                    TextBlock.text "Quiz history, grades, and pass/fail status"
                                    TextBlock.fontSize 12.0
                                    TextBlock.foreground Theme.textSecondary
                                ]
                            ]
                        ]

                        Border.create [
                            Border.padding (14.0, 6.0)
                            Border.child (
                                Grid.create [
                                    Grid.columnDefinitions columnDefs
                                    Grid.columnSpacing 12.0
                                    Grid.children [
                                        headerCell 0 "QUIZ DATE"
                                        headerCell 1 "QUIZ TITLE"
                                        headerCell 2 "GRADE"
                                        headerCell 3 "PASS / FAIL"
                                    ]
                                ]
                            )
                        ]

                        if exams.IsEmpty then
                            Border.create [
                                Border.background Theme.bgInput
                                Border.borderBrush Theme.borderSubtle
                                Border.borderThickness 1.0
                                Border.cornerRadius 8.0
                                Border.padding 18.0
                                Border.child (
                                    TextBlock.create [
                                        TextBlock.text "No quizzes recorded yet. Register with seeded email one@gmail.com to view Ali's pre-seeded exam."
                                        TextBlock.fontSize 13.0
                                        TextBlock.foreground Theme.textSecondary
                                        TextBlock.textWrapping TextWrapping.Wrap
                                    ]
                                )
                            ] :> IView
                        else
                            StackPanel.create [
                                StackPanel.spacing 8.0
                                StackPanel.children (exams |> List.map examRow)
                            ] :> IView
                    ]
                ]
            )
        ] :> IView
