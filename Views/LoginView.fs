namespace StudentGradesApp.Views

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.Input
open Avalonia.Layout
open Avalonia.Media
open StudentGradesApp

module LoginView =

    let view (appState: IWritable<AppState>) : Component =
        Component(fun ctx ->
            let email = ctx.useState ""
            let password = ctx.useState ""
            let showPassword = ctx.useState false
            let errorMessage = ctx.useState<string option> None

            let handleLogin () =
                match State.loginUser email.Current password.Current appState.Current with
                | Ok nextState ->
                    errorMessage.Set None
                    appState.Set nextState
                | Error err -> errorMessage.Set(Some err)

            let registeredHint =
                match appState.Current.RegisteredUser with
                | Some user -> sprintf "Sign in with your registered email (%s) and password." user.Email
                | None -> "Enter your registered email and password."

            Border.create [
                Border.background Theme.bgCanvas
                Border.padding 24.0
                Border.child (
                    Border.create [
                        Border.maxWidth 460.0
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
                                            Theme.badge "STEP 2 OF 2" Theme.accentSoft "#C7D2FE"
                                            TextBlock.create [
                                                TextBlock.text "Welcome Back"
                                                TextBlock.fontSize 24.0
                                                TextBlock.fontWeight FontWeight.Bold
                                                TextBlock.foreground Theme.textPrimary
                                            ]
                                            TextBlock.create [
                                                TextBlock.text registeredHint
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

                                    StackPanel.create [
                                        StackPanel.spacing 10.0
                                        StackPanel.margin (0.0, 6.0, 0.0, 0.0)
                                        StackPanel.onKeyDown (fun e ->
                                            if e.Key = Key.Enter then
                                                handleLogin ()
                                                e.Handled <- true)
                                        StackPanel.children [
                                            Button.create [
                                                Button.padding (16.0, 12.0)
                                                Button.cornerRadius 8.0
                                                Button.horizontalAlignment HorizontalAlignment.Stretch
                                                Button.horizontalContentAlignment HorizontalAlignment.Center
                                                Button.background Theme.accentPrimary
                                                Button.foreground "#FFFFFF"
                                                Button.fontSize 15.0
                                                Button.fontWeight FontWeight.SemiBold
                                                Button.content "Sign In"
                                                Button.onClick (fun _ -> handleLogin ())
                                            ]
                                            Button.create [
                                                Button.padding (14.0, 10.0)
                                                Button.cornerRadius 8.0
                                                Button.horizontalAlignment HorizontalAlignment.Stretch
                                                Button.horizontalContentAlignment HorizontalAlignment.Center
                                                Button.background Theme.bgElevated
                                                Button.foreground Theme.textSecondary
                                                Button.borderBrush Theme.borderSubtle
                                                Button.borderThickness 1.0
                                                Button.content "Register a Different Account"
                                                Button.onClick (fun _ -> appState.Set(State.goToRegistration appState.Current))
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        )
                    ]
                )
            ]
        )
