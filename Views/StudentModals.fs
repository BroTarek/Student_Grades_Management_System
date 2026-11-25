namespace StudentGradesApp.Views

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Input
open Avalonia.Layout
open Avalonia.Media
open StudentGradesApp

module StudentModals =

    let private gradeOptions = [ 1 .. 12 ]

    let view (appState: IWritable<AppState>) (editingStudent: StudentRecord option) : Component =
        Component(fun ctx ->
            let initialName = editingStudent |> Option.map (fun s -> s.Name) |> Option.defaultValue ""
            let initialEmail = editingStudent |> Option.map (fun s -> s.Email) |> Option.defaultValue ""
            let initialGrade = editingStudent |> Option.map (fun s -> s.Grade) |> Option.defaultValue 1

            let name = ctx.useState initialName
            let email = ctx.useState initialEmail
            let grade = ctx.useState initialGrade
            let errorMessage = ctx.useState<string option> None

            let isEdit = Option.isSome editingStudent
            let title = if isEdit then "Edit Student Info" else "Add New Student"
            let subtitle =
                if isEdit then "Update the selected student's profile details."
                else "Enter the student's details to add them to the directory."
            let submitLabel = if isEdit then "Save Changes" else "Add Student"

            let closeModal () =
                appState.Set { appState.Current with ActiveModal = NoModal }

            let handleSubmit () =
                let result =
                    match editingStudent with
                    | Some student -> State.updateStudent student.Id name.Current email.Current grade.Current appState.Current
                    | None -> State.addStudent name.Current email.Current grade.Current appState.Current

                match result with
                | Ok nextState ->
                    errorMessage.Set None
                    appState.Set nextState
                | Error err -> errorMessage.Set(Some err)

            Border.create [
                Border.background "#B3020617"
                Border.padding 24.0
                Border.child (
                    Border.create [
                        Border.maxWidth 460.0
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
                                        e.Handled <- true
                                    elif e.Key = Key.Enter then
                                        handleSubmit ()
                                        e.Handled <- true)
                                StackPanel.children [
                                    StackPanel.create [
                                        StackPanel.spacing 4.0
                                        StackPanel.children [
                                            TextBlock.create [
                                                TextBlock.text title
                                                TextBlock.fontSize 20.0
                                                TextBlock.fontWeight FontWeight.Bold
                                                TextBlock.foreground Theme.textPrimary
                                            ]
                                            TextBlock.create [
                                                TextBlock.text subtitle
                                                TextBlock.fontSize 13.0
                                                TextBlock.foreground Theme.textSecondary
                                                TextBlock.textWrapping TextWrapping.Wrap
                                            ]
                                        ]
                                    ]

                                    Theme.errorBanner errorMessage.Current

                                    StackPanel.create [
                                        StackPanel.spacing 6.0
                                        StackPanel.children [
                                            Theme.fieldLabel "Student Name"
                                            Theme.styledTextBox "Enter student name" name.Current name.Set
                                        ]
                                    ]

                                    StackPanel.create [
                                        StackPanel.spacing 6.0
                                        StackPanel.children [
                                            Theme.fieldLabel "Email"
                                            Theme.styledTextBox "student@example.com" email.Current email.Set
                                        ]
                                    ]

                                    StackPanel.create [
                                        StackPanel.spacing 6.0
                                        StackPanel.children [
                                            Theme.fieldLabel "Grade (1 - 12)"
                                            ComboBox.create [
                                                ComboBox.dataItems gradeOptions
                                                ComboBox.selectedItem grade.Current
                                                ComboBox.horizontalAlignment HorizontalAlignment.Stretch
                                                ComboBox.background Theme.bgInput
                                                ComboBox.foreground Theme.textPrimary
                                                ComboBox.borderBrush Theme.borderSubtle
                                                ComboBox.onSelectedItemChanged (fun item ->
                                                    match item with
                                                    | :? int as g -> grade.Set g
                                                    | _ -> ())
                                            ]
                                        ]
                                    ]

                                    StackPanel.create [
                                        StackPanel.orientation Orientation.Horizontal
                                        StackPanel.horizontalAlignment HorizontalAlignment.Right
                                        StackPanel.spacing 10.0
                                        StackPanel.margin (0.0, 6.0, 0.0, 0.0)
                                        StackPanel.children [
                                            Theme.secondaryButton "Cancel" true closeModal
                                            Theme.primaryButton submitLabel true handleSubmit
                                        ]
                                    ]
                                ]
                            ]
                        )
                    ]
                )
            ]
        )
