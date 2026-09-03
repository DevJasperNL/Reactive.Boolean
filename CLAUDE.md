# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`Reactive.Boolean` is a NuGet library of Rx.NET extension methods for `IObservable<bool>`: logical operators (`And`/`Or`/`Xor` plus `Nand`/`Nor`/`Xnor`/`Not`), time-based "scheduling" operators (`TrueForAtLeast`, `PersistTrueFor`, `WhenTrueFor`, `WhenStableFor`, `LimitTrueDuration`, `PulseTrueFor`, `BlinkWhileTrue` and their `False` mirrors), and `SubscribeTrue`/`SubscribeFalse`/`SubscribeTrueFalse` helpers. Everything public lives on the single `partial static class BooleanObservableExtensions` in namespace `Reactive.Boolean`, split across `BooleanObservableExtensions.*.cs` files. Only dependency: `System.Reactive`.

## Commands

Targets `net8.0;net9.0;net10.0` for both the library and the tests, so every `dotnet test` runs the suite three times unless you pass `-f`.

```powershell
dotnet build --configuration Release -p:TreatWarningsAsErrors=true   # what CI runs; warnings fail the build
dotnet test                                                          # all TFMs
dotnet test -f net10.0                                               # one TFM (much faster while iterating)
dotnet test -f net10.0 --filter "FullyQualifiedName~PersistTrueFor"  # one test class (class names match file names, minus dots)
dotnet test -f net10.0 --filter "Name~TrueForAtLeast_TrueThenFalse_SameForEverySourceShape"  # one test method (all DataRows; use ~ not =, data-driven names include the arguments)
```

`GenerateDocumentationFile` is on and CI treats warnings as errors, so every new public member needs an XML doc comment.

## Architecture

### Scheduling operators are three state machines behind one base class

All time-based operators are built on `TimedBooleanOperator` (`src/Reactive.Boolean/TimedBooleanOperator.cs`). It subscribes to the source exactly once, owns a single restartable timer on the caller's `IScheduler`, and serialises source values, timer expiry, completion and disposal under one lock. Timer callbacks carry a generation number so a stale callback that was already dequeued when the timer was restarted is ignored. Subclasses only implement `OnSourceValue` / `OnTimerElapsed` and use `Emit`, `StartTimer`, `StopTimer`, `LastSourceValue`, `LastEmittedValue`, `TimerRunning`.

Three concrete machines cover all public operators; the public methods in `BooleanObservableExtensions.Scheduling.cs` are thin `Observable.Create` wrappers that pick a machine and its flags:

| Machine | Flags | Public operators |
|---|---|---|
| `TimedWindowOperator` | `withholdFalseDuringWindow`, `forceFalseAtEnd` | `TrueForAtLeast` (withhold), `LimitTrueDuration` (force), `PulseTrueFor` (both) |
| `DelayedTransitionOperator` | `delayTrue`, `delayFalse`, `assumedInitialValue` | `WhenTrueFor` (delay true, assume initial false), `PersistTrueFor` (delay false), `WhenStableFor` (delay both) |
| `BlinkWhileTrueOperator` | `onDuration`, `offDuration` | `BlinkWhileTrue` |

Every `False` variant is literally `source.Not().TrueVariant(...).Not()`; do not implement them separately.

Cross-cutting options shared by every scheduling method: `distinctUntilChanged` (handled centrally in `Emit`), `resetTimerOnConsecutive*` (whether a repeated value restarts a running timer), and `CompletionBehavior` (`CompleteImmediately` vs `CompleteAfterTimer`; the latter relies on each machine's `HasPendingValue` override to decide whether there is anything worth waiting for).

### Logical operators

`And`/`Or` are `CombineLatest` with `OperatorDistinctness` deciding where `DistinctUntilChanged` is applied (output, inputs, or nowhere). Each has many overloads (two observables, `IEnumerable`, `params`, 3- and 4-arity) plus `AndOp`/`OrOp` aliases that exist only to dodge name clashes with Rx join patterns. When adding an overload, add it to the alias and the inverted (`Nand`/`Nor`) families too.

### Tests

MSTest with `Microsoft.Reactive.Testing.TestScheduler`; time is advanced with `scheduler.AdvanceBy(ticks)` and tests typically use `TimeSpan.FromTicks(2)` so each tick is observable. Tests run in parallel at method level (`MSTestSettings.cs`).

`tests/Reactive.Boolean.Tests/SourceShapes.cs` defines `SourceShape` (hot `Subject`, `SelectMany` burst, cold array, cold `Concat`, `Defer`+`Prepend`). The `*_SameForEverySourceShape` tests assert that an operator behaves identically whether values arrive one at a time or re-entrantly in a burst, and whether or not the source completes. Any change to a scheduling operator should keep those passing, and new scheduling behaviour should get its own shape-parameterised test.

Test file naming mirrors the source: `BooleanObservableExtensions.Scheduling.<Operator>.Tests.cs` holds class `BooleanObservableExtensionsScheduling<Operator>Tests`.

## Conventions

- README.md is packed into the NuGet package and is the user-facing documentation. When adding or changing operator behaviour, update the matching README section (and its marble diagram in `img/` if one exists) in the same change.
- Sources use CRLF line endings; files under `src/` also start with a UTF-8 BOM
- PRs to `main` must carry at least one `pr:` label (`pr: bugfix`, `pr: new-feature`, `pr: enhancement`, `pr: breaking change`, `pr: documentation`, `pr: dependency-update`). Release-drafter builds the release notes from PR titles grouped by these labels, and publishing a GitHub release triggers the NuGet push with the tag as the package version.
