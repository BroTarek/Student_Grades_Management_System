namespace TextDisplayApp

open System
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI.Types
open Avalonia.Input
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Themes.Fluent

module MainView =

    let view () =
        Component(fun ctx ->
            let draftText = ctx.useState ""
            let submittedText = ctx.useState<string option> None

            let submit () =
                let trimmed = draftText.Current.Trim()
                if not (String.IsNullOrEmpty trimmed) then
                    submittedText.Set(Some trimmed)

            let clear () =
                draftText.Set ""
                submittedText.Set None

            Border.create [
                Border.background "#0F172A"
                Border.padding 28.0
                Border.child (
                    Border.create [
                        Border.background "#1E293B"
                        Border.borderBrush "#334155"
                        Border.borderThickness 1.0
                        Border.cornerRadius 16.0
                        Border.padding 28.0
                        Border.verticalAlignment VerticalAlignment.Center
                        Border.child (
                            StackPanel.create [
                                StackPanel.spacing 18.0
                                StackPanel.children [
                                    StackPanel.create [
                                        StackPanel.spacing 4.0
                                        StackPanel.children [
                                            TextBlock.create [
                                                TextBlock.text "F# Text Display"
                                                TextBlock.fontSize 24.0
                                                TextBlock.fontWeight FontWeight.Bold
                                                TextBlock.foreground "#F8FAFC"
                                            ]
                                            TextBlock.create [
                                                TextBlock.text "Enter text below and submit to display it on screen."
                                                TextBlock.fontSize 13.0
                                                TextBlock.foreground "#94A3B8"
                                            ]
                                        ]
                                    ]

                                    Grid.create [
                                        Grid.columnDefinitions "*, Auto, Auto"
                                        Grid.children [
                                            TextBox.create [
                                                Grid.column 0
                                                TextBox.text draftText.Current
                                                TextBox.onTextChanged draftText.Set
                                                TextBox.placeHolderText "Type your text here..."
                                                TextBox.fontSize 14.0
                                                TextBox.padding (12.0, 10.0)
                                                TextBox.cornerRadius 8.0
                                                TextBox.background "#0F172A"
                                                TextBox.foreground "#F8FAFC"
                                                TextBox.borderBrush "#475569"
                                                TextBox.onKeyDown (fun e ->
                                                    if e.Key = Key.Enter then
                                                        submit ()
                                                        e.Handled <- true)
                                            ]

                                            Button.create [
                                                Grid.column 1
                                                Button.margin (10.0, 0.0, 0.0, 0.0)
                                                Button.padding (18.0, 10.0)
                                                Button.cornerRadius 8.0
                                                Button.background "#6366F1"
                                                Button.foreground "#FFFFFF"
                                                Button.fontWeight FontWeight.SemiBold
                                                Button.content "Submit"
                                                Button.isEnabled (not (String.IsNullOrWhiteSpace draftText.Current))
                                                Button.onClick (fun _ -> submit ())
                                            ]

                                            Button.create [
                                                Grid.column 2
                                                Button.margin (8.0, 0.0, 0.0, 0.0)
                                                Button.padding (14.0, 10.0)
                                                Button.cornerRadius 8.0
                                                Button.background "#334155"
                                                Button.foreground "#E2E8F0"
                                                Button.content "Clear"
                                                Button.onClick (fun _ -> clear ())
                                            ]
                                        ]
                                    ]

                                    Border.create [
                                        Border.background "#0F172A"
                                        Border.borderBrush (
                                            match submittedText.Current with
                                            | Some _ -> "#6366F1"
                                            | None -> "#334155"
                                        )
                                        Border.borderThickness 1.5
                                        Border.cornerRadius 12.0
                                        Border.padding 20.0
                                        Border.minHeight 120.0
                                        Border.child (
                                            match submittedText.Current with
                                            | Some text ->
                                                StackPanel.create [
                                                    StackPanel.spacing 8.0
                                                    StackPanel.children [
                                                        TextBlock.create [
                                                            TextBlock.text "SUBMITTED TEXT"
                                                            TextBlock.fontSize 11.0
                                                            TextBlock.fontWeight FontWeight.SemiBold
                                                            TextBlock.foreground "#818CF8"
                                                        ]
                                                        SelectableTextBlock.create [
                                                            SelectableTextBlock.text text
                                                            SelectableTextBlock.fontSize 18.0
                                                            SelectableTextBlock.fontWeight FontWeight.Medium
                                                            SelectableTextBlock.foreground "#F8FAFC"
                                                            SelectableTextBlock.textWrapping TextWrapping.Wrap
                                                        ]
                                                    ]
                                                ] :> IView
                                            | None ->
                                                TextBlock.create [
                                                    TextBlock.text "No text submitted yet."
                                                    TextBlock.fontSize 14.0
                                                    TextBlock.foreground "#64748B"
                                                    TextBlock.horizontalAlignment HorizontalAlignment.Center
                                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                                ] :> IView
                                        )
                                    ]
                                ]
                            ]
                        )
                    ]
                )
            ]
        )

type MainWindow() as this =
    inherit HostWindow()

    do
        base.Title <- "F# Desktop Text Display"
        base.Width <- 600.0
        base.Height <- 380.0
        base.MinWidth <- 460.0
        base.MinHeight <- 320.0
        base.WindowStartupLocation <- WindowStartupLocation.CenterScreen
        this.Content <- MainView.view ()

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)
