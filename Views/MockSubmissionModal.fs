namespace StudentGradesApp.Views

open System
open System.Globalization
open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Input
open Avalonia.Layout
open Avalonia.Media
open StudentGradesApp

module MockSubmissionModal =

    let view (appState: IWritable<AppState>) : Component =
        Component(fun ctx ->
            let gradeInput = ctx.useState ""
            let errorMessage = ctx.useState<string option> None
            let submission = Domain.constantMockSubmission

            let closeModal () =
                appState.Set { appState.Current with ActiveModal = NoModal }

            let handleSubmit () =
                let mutable parsed = 0.0
                if Double.TryParse(gradeInput.Current.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, &parsed) then
                    match State.submitMockGrade parsed appState.Current with
                    | Ok nextState ->
                        errorMessage.Set None
                        appState.Set nextState
                    | Error err -> errorMessage.Set(Some err)
                else
                    errorMessage.Set(Some "Please enter a valid numeric grade between 0 and 100.")

            Border.create [
                Border.background "#B3020617"
                Border.padding 24.0
                Border.child (
                    Border.create [
                        Border.maxWidth 600.0
                        Border.horizontalAlignment HorizontalAlignment.Center
                        Border.verticalAlignment VerticalAlignment.Center
                        Border.background Theme.bgCard
                        Border.borderBrush Theme.borderAccent
                        Border.borderThickness 1.5
                        Border.cornerRadius 16.0
                        Border.padding 28.0
                        Border.child (
                            StackPanel.create [
                                StackPanel.spacing 16.0
                                StackPanel.onKeyDown (fun e ->
                                    if e.Key = Key.Escape then
                                        closeModal ()
                                        e.Handled <- true)
                                StackPanel.children [
                                    StackPanel.create [
                                        StackPanel.spacing 6.0
                                        StackPanel.children [
                                            Theme.badge "MOCK STUDENT SUBMISSION" Theme.accentSoft "#C7D2FE"
                                            TextBlock.create [
                                                TextBlock.text submission.QuizTitle
                                                TextBlock.fontSize 20.0
                                                TextBlock.fontWeight FontWeight.Bold
                                                TextBlock.foreground Theme.textPrimary
                                                TextBlock.textWrapping TextWrapping.Wrap
                                            ]
                                        ]
                                    ]

                                    Border.create [
                                        Border.background Theme.bgInput
                                        Border.borderBrush Theme.borderSubtle
                                        Border.borderThickness 1.0
                                        Border.cornerRadius 10.0
                                        Border.padding 14.0
                                        Border.child (
                                            Grid.create [
                                                Grid.columnDefinitions "*, *, *, *"
                                                Grid.columnSpacing 10.0
                                                Grid.children [
                                                    StackPanel.create [
                                                        Grid.column 0
                                                        StackPanel.spacing 2.0
                                                        StackPanel.children [
                                                            TextBlock.create [ TextBlock.text "STUDENT"; TextBlock.fontSize 10.0; TextBlock.foreground Theme.textMuted; TextBlock.fontWeight FontWeight.Bold ]
                                                            TextBlock.create [ TextBlock.text submission.StudentName; TextBlock.fontSize 13.0; TextBlock.foreground Theme.textPrimary; TextBlock.fontWeight FontWeight.SemiBold ]
                                                        ]
                                                    ]
                                                    StackPanel.create [
                                                        Grid.column 1
                                                        StackPanel.spacing 2.0
                                                        StackPanel.children [
                                                            TextBlock.create [ TextBlock.text "EMAIL"; TextBlock.fontSize 10.0; TextBlock.foreground Theme.textMuted; TextBlock.fontWeight FontWeight.Bold ]
                                                            TextBlock.create [ TextBlock.text submission.StudentEmail; TextBlock.fontSize 12.0; TextBlock.foreground Theme.textSecondary; TextBlock.textWrapping TextWrapping.Wrap ]
                                                        ]
                                                    ]
                                                    StackPanel.create [
                                                        Grid.column 2
                                                        StackPanel.spacing 2.0
                                                        StackPanel.children [
                                                            TextBlock.create [ TextBlock.text "GRADE"; TextBlock.fontSize 10.0; TextBlock.foreground Theme.textMuted; TextBlock.fontWeight FontWeight.Bold ]
                                                            TextBlock.create [ TextBlock.text (sprintf "Grade %d" submission.Grade); TextBlock.fontSize 13.0; TextBlock.foreground Theme.textPrimary; TextBlock.fontWeight FontWeight.SemiBold ]
                                                        ]
                                                    ]
                                                    StackPanel.create [
                                                        Grid.column 3
                                                        StackPanel.spacing 2.0
                                                        StackPanel.children [
                                                            TextBlock.create [ TextBlock.text "DATE"; TextBlock.fontSize 10.0; TextBlock.foreground Theme.textMuted; TextBlock.fontWeight FontWeight.Bold ]
                                                            TextBlock.create [ TextBlock.text submission.QuizDate; TextBlock.fontSize 13.0; TextBlock.foreground Theme.textSecondary ]
                                                        ]
                                                    ]
                                                ]
                                            ]
                                        )
                                    ]

                                    StackPanel.create [
                                        StackPanel.spacing 6.0
                                        StackPanel.children [
                                            Theme.fieldLabel "Submitted Essay"
                                            Border.create [
                                                Border.background Theme.bgInput
                                                Border.borderBrush Theme.borderSubtle
                                                Border.borderThickness 1.0
                                                Border.cornerRadius 10.0
                                                Border.padding 14.0
                                                Border.maxHeight 160.0
                                                Border.child (
                                                    ScrollViewer.create [
                                                        ScrollViewer.content (
                                                            TextBlock.create [
                                                                TextBlock.text submission.EssayText
                                                                TextBlock.fontSize 13.0
                                                                TextBlock.lineHeight 20.0
                                                                TextBlock.foreground "#E2E8F0"
                                                                TextBlock.textWrapping TextWrapping.Wrap
                                                            ]
                                                        )
                                                    ]
                                                )
                                            ]
                                        ]
                                    ]

                                    Theme.errorBanner errorMessage.Current

                                    StackPanel.create [
                                        StackPanel.spacing 6.0
                                        StackPanel.children [
                                            Theme.fieldLabel "Assign Grade (0 - 100, Pass >= 50)"
                                            Theme.styledTextBox "e.g. 85" gradeInput.Current gradeInput.Set
                                        ]
                                    ]

                                    StackPanel.create [
                                        StackPanel.orientation Orientation.Horizontal
                                        StackPanel.horizontalAlignment HorizontalAlignment.Right
                                        StackPanel.spacing 10.0
                                        StackPanel.children [
                                            Theme.secondaryButton "Cancel" true closeModal
                                            Theme.primaryButton "Submit Grade" true handleSubmit
                                        ]
                                    ]
                                ]
                            ]
                        )
                    ]
                )
            ]
        )
