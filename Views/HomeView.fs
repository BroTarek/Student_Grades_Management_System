namespace StudentGradesApp.Views

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open StudentGradesApp

module HomeView =

    let private topBar (appState: IWritable<AppState>) (user: RegisteredUser) : IView =
        Border.create [
            Border.background Theme.bgCard
            Border.borderBrush Theme.borderSubtle
            Border.borderThickness (0.0, 0.0, 0.0, 1.0)
            Border.padding (24.0, 14.0)
            Border.child (
                DockPanel.create [
                    DockPanel.children [
                        StackPanel.create [
                            DockPanel.dock Dock.Left
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.spacing 12.0
                            StackPanel.verticalAlignment VerticalAlignment.Center
                            StackPanel.children [
                                TextBlock.create [
                                    TextBlock.text "Students Grade Management"
                                    TextBlock.fontSize 18.0
                                    TextBlock.fontWeight FontWeight.Bold
                                    TextBlock.foreground Theme.textPrimary
                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                ]
                                Theme.badge
                                    (sprintf "Role: %s" (Domain.roleShortName user.Role))
                                    Theme.accentSoft
                                    "#C7D2FE"
                            ]
                        ]

                        StackPanel.create [
                            DockPanel.dock Dock.Right
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.horizontalAlignment HorizontalAlignment.Right
                            StackPanel.spacing 10.0
                            StackPanel.children [
                                Theme.secondaryButton "Register New Account" true (fun () ->
                                    appState.Set(State.goToRegistration appState.Current))
                                Theme.secondaryButton "Sign Out" true (fun () ->
                                    appState.Set(State.logout appState.Current))
                            ]
                        ]
                    ]
                ]
            )
        ] :> IView

    let view (appState: IWritable<AppState>) : IView =
        match appState.Current.LoggedInUser with
        | None ->
            Border.create [
                Border.background Theme.bgCanvas
                Border.child (
                    TextBlock.create [
                        TextBlock.text "Session expired. Please sign in."
                        TextBlock.foreground Theme.textPrimary
                        TextBlock.horizontalAlignment HorizontalAlignment.Center
                        TextBlock.verticalAlignment VerticalAlignment.Center
                    ]
                )
            ] :> IView
        | Some user ->
            let isAdmin = Domain.isAdmin user.Role
            let matchedStudent = State.findStudentForUser user appState.Current.Students

            let mainContent =
                DockPanel.create [
                    DockPanel.background Theme.bgCanvas
                    DockPanel.children [
                        Border.create [
                            DockPanel.dock Dock.Top
                            Border.child (topBar appState user)
                        ]
                        ScrollViewer.create [
                            ScrollViewer.content (
                                StackPanel.create [
                                    StackPanel.maxWidth 1060.0
                                    StackPanel.horizontalAlignment HorizontalAlignment.Stretch
                                    StackPanel.margin 24.0
                                    StackPanel.spacing 20.0
                                    StackPanel.children [
                                        ProfilePanel.view user matchedStudent

                                        if not isAdmin then
                                            ExamsTableView.view matchedStudent

                                        StudentsTableView.view appState isAdmin
                                    ]
                                ]
                            )
                        ]
                    ]
                ] :> IView

            let modalOverlay : IView list =
                match appState.Current.ActiveModal with
                | NoModal -> []
                | AddStudentModal ->
                    [ ContentControl.create [
                          ContentControl.content (StudentModals.view appState None)
                      ] :> IView ]
                | EditStudentModal student ->
                    [ ContentControl.create [
                          ContentControl.content (StudentModals.view appState (Some student))
                      ] :> IView ]
                | MockSubmissionModal ->
                    [ ContentControl.create [
                          ContentControl.content (MockSubmissionModal.view appState)
                      ] :> IView ]

            Grid.create [
                Grid.children (mainContent :: modalOverlay)
            ] :> IView
