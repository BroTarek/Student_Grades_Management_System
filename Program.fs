namespace StudentGradesApp

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Hosts
open Avalonia.Themes.Fluent
open StudentGradesApp.Views

module RootView =

    let view () : Component =
        Component(fun ctx ->
            let appState = ctx.useState State.initialState

            match appState.Current.CurrentScreen with
            | Registration ->
                ContentControl.create [
                    ContentControl.content (RegistrationView.view appState)
                ]
            | Login ->
                ContentControl.create [
                    ContentControl.content (LoginView.view appState)
                ]
            | Home ->
                ContentControl.create [
                    ContentControl.content (HomeView.view appState)
                ]
        )

type MainWindow() as this =
    inherit HostWindow()

    do
        base.Title <- "Students Grade Management"
        base.Width <- 1080.0
        base.Height <- 740.0
        base.MinWidth <- 820.0
        base.MinHeight <- 580.0
        base.WindowStartupLocation <- WindowStartupLocation.CenterScreen
        this.Content <- RootView.view ()

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
        if args |> Array.contains "--test" then
            Tests.runAll ()
        else
            AppBuilder
                .Configure<App>()
                .UsePlatformDetect()
                .UseSkia()
                .StartWithClassicDesktopLifetime(args)
