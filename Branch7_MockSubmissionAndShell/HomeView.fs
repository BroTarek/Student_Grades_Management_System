namespace StudentGradesApp.Views

open Avalonia.Controls
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open StudentGradesApp

module HomeView =

    let private highlightTile
        (title: string)
        (badgeBg: string)
        (badgeFg: string)
        (borderAccent: string)
        (entry: (StudentRecord * float) option)
        : IView =
        Border.create [
            Border.background Theme.bgInput
            Border.borderBrush borderAccent
            Border.borderThickness 1.2
            Border.cornerRadius 12.0
            Border.padding 16.0
            Border.child (
                match entry with
                | Some (student, avg) ->
                    let stats = Domain.calculateStatistics student.Exams
                    let statsBg = if stats = Good then Theme.successBg else Theme.dangerBg
                    let statsFg = if stats = Good then Theme.successText else Theme.dangerText

                    StackPanel.create [
                        StackPanel.spacing 8.0
                        StackPanel.children [
                            Theme.badge title badgeBg badgeFg
                            TextBlock.create [
                                TextBlock.text student.Name
                                TextBlock.fontSize 17.0
                                TextBlock.fontWeight FontWeight.Bold
                                TextBlock.foreground Theme.textPrimary
                                TextBlock.textWrapping TextWrapping.Wrap
                            ]
                            TextBlock.create [
                                TextBlock.text (sprintf "Grade %d • %s" student.Grade student.Email)
                                TextBlock.fontSize 12.0
                                TextBlock.foreground Theme.textSecondary
                                TextBlock.textWrapping TextWrapping.Wrap
                            ]
                            DockPanel.create [
                                DockPanel.margin (0.0, 4.0, 0.0, 0.0)
                                DockPanel.children [
                                    StackPanel.create [
                                        DockPanel.dock Dock.Left
                                        StackPanel.spacing 2.0
                                        StackPanel.children [
                                            TextBlock.create [
                                                TextBlock.text "AVERAGE"
                                                TextBlock.fontSize 10.0
                                                TextBlock.fontWeight FontWeight.Bold
                                                TextBlock.foreground Theme.textMuted
                                            ]
                                            TextBlock.create [
                                                TextBlock.text (sprintf "%.1f%%" avg)
                                                TextBlock.fontSize 22.0
                                                TextBlock.fontWeight FontWeight.Bold
                                                TextBlock.foreground badgeFg
                                            ]
                                        ]
                                    ]
                                    StackPanel.create [
                                        DockPanel.dock Dock.Right
                                        StackPanel.horizontalAlignment HorizontalAlignment.Right
                                        StackPanel.verticalAlignment VerticalAlignment.Bottom
                                        StackPanel.spacing 4.0
                                        StackPanel.children [
                                            Theme.badge (Domain.statusText stats) statsBg statsFg
                                            TextBlock.create [
                                                TextBlock.text (sprintf "%d exam(s)" student.Exams.Length)
                                                TextBlock.fontSize 11.0
                                                TextBlock.foreground Theme.textMuted
                                                TextBlock.horizontalAlignment HorizontalAlignment.Right
                                            ]
                                        ]
                                    ]
                                ]
                            ]
                        ]
                    ]
                | None ->
                    StackPanel.create [
                        StackPanel.spacing 6.0
                        StackPanel.children [
                            Theme.badge title badgeBg badgeFg
                            TextBlock.create [
                                TextBlock.text "No student data available."
                                TextBlock.fontSize 13.0
                                TextBlock.foreground Theme.textSecondary
                            ]
                        ]
                    ]
            )
        ] :> IView

    let private teacherAverageSummaryCard (students: StudentRecord list) : IView =
        let highest, lowest = Domain.highestAndLowestAverageStudents students

        Border.create [
            Border.background Theme.bgCard
            Border.borderBrush Theme.borderSubtle
            Border.borderThickness 1.0
            Border.cornerRadius 14.0
            Border.padding 20.0
            Border.verticalAlignment VerticalAlignment.Top
            Border.child (
                StackPanel.create [
                    StackPanel.spacing 14.0
                    StackPanel.children [
                        StackPanel.create [
                            StackPanel.spacing 4.0
                            StackPanel.children [
                                Theme.badge "CLASS EXTREMES" Theme.accentSoft "#C7D2FE"
                                TextBlock.create [
                                    TextBlock.text "Average Highlights"
                                    TextBlock.fontSize 18.0
                                    TextBlock.fontWeight FontWeight.Bold
                                    TextBlock.foreground Theme.textPrimary
                                ]
                                TextBlock.create [
                                    TextBlock.text "Highest and lowest student averages across current records."
                                    TextBlock.fontSize 12.0
                                    TextBlock.foreground Theme.textSecondary
                                    TextBlock.textWrapping TextWrapping.Wrap
                                ]
                            ]
                        ]

                        highlightTile "HIGHEST AVERAGE" Theme.successBg Theme.successText "#10B981" highest
                        highlightTile "LOWEST AVERAGE" Theme.dangerBg Theme.dangerText "#F43F5E" lowest
                    ]
                ]
            )
        ] :> IView

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

            let leftColumn =
                StackPanel.create [
                    Grid.column 0
                    StackPanel.spacing 20.0
                    StackPanel.children [
                        ProfilePanel.view user matchedStudent

                        if not isAdmin then
                            ExamsTableView.view matchedStudent

                        StudentsTableView.view appState isAdmin
                    ]
                ] :> IView

            let dashboardGrid =
                if isAdmin then
                    Grid.create [
                        Grid.maxWidth 1320.0
                        Grid.horizontalAlignment HorizontalAlignment.Stretch
                        Grid.margin 24.0
                        Grid.columnDefinitions "*, 310"
                        Grid.columnSpacing 20.0
                        Grid.children [
                            leftColumn
                            Border.create [
                                Grid.column 1
                                Border.child (teacherAverageSummaryCard appState.Current.Students)
                            ] :> IView
                        ]
                    ] :> IView
                else
                    Grid.create [
                        Grid.maxWidth 1060.0
                        Grid.horizontalAlignment HorizontalAlignment.Stretch
                        Grid.margin 24.0
                        Grid.columnDefinitions "*"
                        Grid.children [ leftColumn ]
                    ] :> IView

            let mainContent =
                DockPanel.create [
                    DockPanel.background Theme.bgCanvas
                    DockPanel.children [
                        Border.create [
                            DockPanel.dock Dock.Top
                            Border.child (topBar appState user)
                        ]
                        ScrollViewer.create [
                            ScrollViewer.content dashboardGrid
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
