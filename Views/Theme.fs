namespace StudentGradesApp.Views

open Avalonia
open Avalonia.Controls
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Avalonia.Layout
open Avalonia.Media

module Theme =

    let bgCanvas = "#0B1120"
    let bgCard = "#131C31"
    let bgElevated = "#1A2540"
    let bgInput = "#0F172A"
    let borderSubtle = "#293548"
    let borderAccent = "#6366F1"

    let textPrimary = "#F8FAFC"
    let textSecondary = "#94A3B8"
    let textMuted = "#64748B"

    let accentPrimary = "#6366F1"
    let accentSoft = "#312E81"
    let successBg = "#064E3B"
    let successText = "#34D399"
    let dangerBg = "#4C0519"
    let dangerText = "#FB7185"
    let dangerButtonBg = "#E11D48"

    let fieldLabel (label: string) : IView =
        TextBlock.create [
            TextBlock.text label
            TextBlock.fontSize 13.0
            TextBlock.fontWeight FontWeight.SemiBold
            TextBlock.foreground "#CBD5E1"
        ] :> IView

    let styledTextBox (placeholder: string) (value: string) (onChange: string -> unit) : IView =
        TextBox.create [
            TextBox.text value
            TextBox.onTextChanged onChange
            TextBox.placeHolderText placeholder
            TextBox.fontSize 14.0
            TextBox.padding (12.0, 10.0)
            TextBox.cornerRadius 8.0
            TextBox.background bgInput
            TextBox.foreground textPrimary
            TextBox.borderBrush borderSubtle
        ] :> IView

    let passwordInputWithToggle
        (placeholder: string)
        (value: string)
        (showPassword: bool)
        (onChange: string -> unit)
        (onToggleShow: unit -> unit)
        : IView =
        Grid.create [
            Grid.columnDefinitions "*, Auto"
            Grid.children [
                TextBox.create [
                    Grid.column 0
                    TextBox.text value
                    TextBox.onTextChanged onChange
                    TextBox.placeHolderText placeholder
                    TextBox.passwordChar (if showPassword then '\000' else '\u2022')
                    TextBox.fontSize 14.0
                    TextBox.padding (12.0, 10.0)
                    TextBox.cornerRadius 8.0
                    TextBox.background bgInput
                    TextBox.foreground textPrimary
                    TextBox.borderBrush borderSubtle
                ]
                Button.create [
                    Grid.column 1
                    Button.margin (8.0, 0.0, 0.0, 0.0)
                    Button.padding (14.0, 10.0)
                    Button.cornerRadius 8.0
                    Button.background bgElevated
                    Button.foreground textPrimary
                    Button.borderBrush borderSubtle
                    Button.borderThickness 1.0
                    Button.fontSize 13.0
                    Button.content (if showPassword then "Hide" else "Show")
                    Button.onClick (fun _ -> onToggleShow ())
                ]
            ]
        ] :> IView

    let errorBanner (message: string option) : IView =
        match message with
        | Some msg ->
            Border.create [
                Border.background dangerBg
                Border.borderBrush dangerText
                Border.borderThickness 1.0
                Border.cornerRadius 8.0
                Border.padding (12.0, 10.0)
                Border.child (
                    TextBlock.create [
                        TextBlock.text msg
                        TextBlock.fontSize 13.0
                        TextBlock.foreground dangerText
                        TextBlock.textWrapping TextWrapping.Wrap
                    ]
                )
            ] :> IView
        | None ->
            Border.create [ Border.isVisible false ] :> IView

    let badge (text: string) (bg: string) (fg: string) : IView =
        Border.create [
            Border.background bg
            Border.cornerRadius 999.0
            Border.padding (10.0, 4.0)
            Border.horizontalAlignment HorizontalAlignment.Left
            Border.child (
                TextBlock.create [
                    TextBlock.text text
                    TextBlock.fontSize 12.0
                    TextBlock.fontWeight FontWeight.SemiBold
                    TextBlock.foreground fg
                ]
            )
        ] :> IView

    let primaryButton (label: string) (enabled: bool) (onClick: unit -> unit) : IView =
        Button.create [
            Button.content label
            Button.isEnabled enabled
            Button.padding (16.0, 10.0)
            Button.cornerRadius 8.0
            Button.background accentPrimary
            Button.foreground "#FFFFFF"
            Button.fontWeight FontWeight.SemiBold
            Button.horizontalContentAlignment HorizontalAlignment.Center
            Button.onClick (fun _ -> onClick ())
        ] :> IView

    let secondaryButton (label: string) (enabled: bool) (onClick: unit -> unit) : IView =
        Button.create [
            Button.content label
            Button.isEnabled enabled
            Button.padding (14.0, 10.0)
            Button.cornerRadius 8.0
            Button.background bgElevated
            Button.foreground textPrimary
            Button.borderBrush borderSubtle
            Button.borderThickness 1.0
            Button.horizontalContentAlignment HorizontalAlignment.Center
            Button.onClick (fun _ -> onClick ())
        ] :> IView
