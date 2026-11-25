namespace StudentGradesApp.Views

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open StudentGradesApp

module RegistrationView =

    let private roleOptions = [ "Student"; "Teacher" ]
    let private gradeOptions = [ 1 .. 12 ]

    let view (appState: IWritable<AppState>) : Component =
        Component(fun ctx ->
            let name = ctx.useState ""
            let selectedRole = ctx.useState "Student"
            let email = ctx.useState ""
            let password = ctx.useState ""
            let showPassword = ctx.useState false
            let selectedGrade = ctx.useState 1
            let errorMessage = ctx.useState<string option> None

            let isStudent = selectedRole.Current = "Student"

            let handleSubmit () =
                let role = Domain.roleFromLabel selectedRole.Current
                let gradeOpt = if isStudent then Some selectedGrade.Current else None

                match State.registerUser name.Current role email.Current password.Current gradeOpt appState.Current with
                | Ok nextState ->
                    errorMessage.Set None
                    appState.Set nextState
                | Error err -> errorMessage.Set(Some err)

            Border.create [
                Border.background Theme.bgCanvas
                Border.padding 24.0
                Border.child (
                    ScrollViewer.create [
                        ScrollViewer.content (
                            Border.create [
                                Border.maxWidth 480.0
                                Border.horizontalAlignment HorizontalAlignment.Center
                                Border.verticalAlignment VerticalAlignment.Center
                                Border.background Theme.bgCard
                                Border.borderBrush Theme.borderSubtle
                                Border.borderThickness 1.0
                                Border.cornerRadius 16.0
                                Border.padding 32.0
                                Border.child (
                                    StackPanel.create [
                                        StackPanel.spacing 16.0
                                        StackPanel.children [
                                            StackPanel.create [
                                                StackPanel.spacing 6.0
                                                StackPanel.children [
                                                    Theme.badge "STEP 1 OF 2" Theme.accentSoft "#C7D2FE"
                                                    TextBlock.create [
                                                        TextBlock.text "Create Your Account"
                                                        TextBlock.fontSize 24.0
                                                        TextBlock.fontWeight FontWeight.Bold
                                                        TextBlock.foreground Theme.textPrimary
                                                    ]
                                                    TextBlock.create [
                                                        TextBlock.text "Register as a Student (Viewer) or Teacher (Admin) to continue."
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
                                                    Theme.fieldLabel "Name"
                                                    Theme.styledTextBox "Enter your full name" name.Current name.Set
                                                ]
                                            ]

                                            StackPanel.create [
                                                StackPanel.spacing 6.0
                                                StackPanel.children [
                                                    Theme.fieldLabel "Role"
                                                    ComboBox.create [
                                                        ComboBox.dataItems roleOptions
                                                        ComboBox.selectedItem selectedRole.Current
                                                        ComboBox.horizontalAlignment HorizontalAlignment.Stretch
                                                        ComboBox.background Theme.bgInput
                                                        ComboBox.foreground Theme.textPrimary
                                                        ComboBox.borderBrush Theme.borderSubtle
                                                        ComboBox.onSelectedItemChanged (fun item ->
                                                            match item with
                                                            | :? string as r -> selectedRole.Set r
                                                            | _ -> ())
                                                    ]
                                                ]
                                            ]

                                            StackPanel.create [
                                                StackPanel.isVisible isStudent
                                                StackPanel.spacing 6.0
                                                StackPanel.children [
                                                    Theme.fieldLabel "Grade (1 - 12)"
                                                    ComboBox.create [
                                                        ComboBox.dataItems gradeOptions
                                                        ComboBox.selectedItem selectedGrade.Current
                                                        ComboBox.horizontalAlignment HorizontalAlignment.Stretch
                                                        ComboBox.background Theme.bgInput
                                                        ComboBox.foreground Theme.textPrimary
                                                        ComboBox.borderBrush Theme.borderSubtle
                                                        ComboBox.onSelectedItemChanged (fun item ->
                                                            match item with
                                                            | :? int as g -> selectedGrade.Set g
                                                            | _ -> ())
                                                    ]
                                                ]
                                            ] :> IView

                                            StackPanel.create [
                                                StackPanel.spacing 6.0
                                                StackPanel.children [
                                                    Theme.fieldLabel "Email"
                                                    Theme.styledTextBox "name@example.com" email.Current email.Set
                                                ]
                                            ]

                                            StackPanel.create [
                                                StackPanel.spacing 6.0
                                                StackPanel.children [
                                                    Theme.fieldLabel "Password"
                                                    Theme.passwordInputWithToggle
                                                        "Enter your password"
                                                        password.Current
                                                        showPassword.Current
                                                        password.Set
                                                        (fun () -> showPassword.Set(not showPassword.Current))
                                                ]
                                            ]

                                            Button.create [
                                                Button.margin (0.0, 6.0, 0.0, 0.0)
                                                Button.padding (16.0, 12.0)
                                                Button.cornerRadius 8.0
                                                Button.horizontalAlignment HorizontalAlignment.Stretch
                                                Button.horizontalContentAlignment HorizontalAlignment.Center
                                                Button.background Theme.accentPrimary
                                                Button.foreground "#FFFFFF"
                                                Button.fontSize 15.0
                                                Button.fontWeight FontWeight.SemiBold
                                                Button.content "Register & Continue to Login"
                                                Button.onClick (fun _ -> handleSubmit ())
                                            ]
                                        ]
                                    ]
                                )
                            ]
                        )
                    ]
                )
            ]
        )
