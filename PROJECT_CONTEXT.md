# Project Context

## Product

Project-x is a 7-inch USB-connected sim-racing touchscreen dashboard with:

- Teensy 4.0 firmware
- RA8775L3N-driven display/rendering
- Windows 11 WPF management app

## Main User Value

The user should be able to:

- connect the device over USB
- complete setup with low friction
- switch modes using a rotary selector
- swipe through fixed themes
- use one replaceable custom theme slot per mode
- switch telemetry source quickly between SimHub and native Project-x telemetry

## Core Modes

- Drift
- GT3
- Formula
- Euro Truck Simulator

## Theme Model

- At least 3 built-in themes per mode in MVP
- Built-in themes are not editable
- One custom theme slot per mode
- The custom theme replaces the last slot for that mode
- If the custom theme is missing or corrupt, revert to the default built-in last slot

## Telemetry Model

Sources:

- SimHub
- Project-x native telemetry

Rules:

- source can be changed on device and in the PC app
- sync is two-way
- conflict resolution is last-write-wins
- switching source changes available theme sets
- show telemetry-unavailable state instead of stale values

## Power And Persistence

- Physical latching on/off switch
- Power can be cut at any time
- Settings must be persisted immediately on change
- On power-up, restore last mode/theme/source

## PC App

Windows app stack:

- C#
- WPF
- MaterialDesignInXaml
- SimHub SDK
- Newtonsoft.Json

The PC app is responsible for:

- USB device detection
- setup and troubleshooting flow
- source selection and sync
- SimHub bridge integration
- native telemetry management
- theme install/select workflow
- diagnostics and logs
- signed updates and integrity verification

## MVP Success Metric

Target:

- 500 successful end-to-end sessions
- less than 2 percent failure rate
- measured within 30 days after MVP release

## Important Scope Boundaries

In scope:

- firmware
- RA8775L3N rendering
- touch calibration
- rotary mode switching
- immediate state persistence
- WPF management app
- diagnostics
- signed updates
- fixed themes plus one custom slot per mode

Out of scope:

- on-device theme editing
- custom widget layouts
- Wi-Fi
- macOS app

