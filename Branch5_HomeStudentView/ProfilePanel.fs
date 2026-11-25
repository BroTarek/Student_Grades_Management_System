namespace StudentGradesApp.Views

open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media
open StudentGradesApp

module ProfilePanel =

    let private infoTile (label: string) (value: string) (accent: string option) : IView =
        Border.create [
            Border.background Theme.bgInput
            Border.borderBrush Theme.borderSubtle
            Border.borderThickness 1.0
            Border.cornerRadius 10.0
            Border.padding (14.0, 10.0)
            Border.child (
                StackPanel.create [
                    StackPanel.spacing 4.0
                    StackPanel.children [
                        TextBlock.create [
                            TextBlock.text label
                            TextBlock.fontSize 11.0
                            TextBlock.fontWeight FontWeight.SemiBold
                            TextBlock.foreground Theme.textMuted
                        ]
                        TextBlock.create [
                            TextBlock.text value
                            TextBlock.fontSize 16.0
                            TextBlock.fontWeight FontWeight.Bold
                            TextBlock.foreground (Option.defaultValue Theme.textPrimary accent)
                            TextBlock.textWrapping TextWrapping.Wrap
                        ]
                    ]
                ]
            )
        ] :> IView

    let view (user: RegisteredUser) (studentRecord: StudentRecord option) : IView =
        let isStudent = Domain.isViewer user.Role

        let averageDisplay =
            match studentRecord with
            | Some student when not student.Exams.IsEmpty ->
                sprintf "%.1f%%" (Domain.calculateAverage student.Exams)
            | _ -> "N/A"

        let gradeDisplay =
            match user.Grade with
            | Some g -> sprintf "Grade %d" g
            | None -> "N/A"

        let roleBadge =
            if Domain.isAdmin user.Role then
                Theme.badge "ADMIN • TEACHER" Theme.accentSoft "#C7D2FE"
            else
                Theme.badge "VIEWER • STUDENT" Theme.successBg Theme.successText

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
                                        TextBlock.create [
                                            TextBlock.text (if isStudent then "Student Profile" else "Teacher Profile")
                                            TextBlock.fontSize 18.0
                                            TextBlock.fontWeight FontWeight.Bold
                                            TextBlock.foreground Theme.textPrimary
                                        ]
                                        TextBlock.create [
                                            TextBlock.text "Submitted account details"
                                            TextBlock.fontSize 12.0
                                            TextBlock.foreground Theme.textSecondary
                                        ]
                                    ]
                                ]
                                Border.create [
                                    DockPanel.dock Dock.Right
                                    Border.horizontalAlignment HorizontalAlignment.Right
                                    Border.verticalAlignment VerticalAlignment.Center
                                    Border.child roleBadge
                                ]
                            ]
                        ]

                        Grid.create [
                            Grid.columnDefinitions (if isStudent then "*, *, *, *, *" else "*, *, *")
                            Grid.columnSpacing 12.0
                            Grid.children [
                                Border.create [
                                    Grid.column 0
                                    Border.child (infoTile "NAME" user.Name None)
                                ]
                                Border.create [
                                    Grid.column 1
                                    Border.child (infoTile "EMAIL" user.Email None)
                                ]
                                Border.create [
                                    Grid.column 2
                                    Border.child (infoTile "ROLE" (Domain.roleDisplayName user.Role) None)
                                ]
                                if isStudent then
                                    Border.create [
                                        Grid.column 3
                                        Border.child (infoTile "GRADE" gradeDisplay None)
                                    ]
                                    Border.create [
                                        Grid.column 4
                                        Border.child (infoTile "AVERAGE" averageDisplay (Some "#818CF8"))
                                    ]
                            ]
                        ]
                    ]
                ]
            )
        ] :> IView
