# Reactive.Boolean

Reactive Extensions meant specifically for implementations of `IObservable<bool>`

This documentation uses marble diagrams to explain the transformations of `IObservable<bool>`. More on marble diagrams can be found in the documentation of [ReactiveX](https://reactivex.io/documentation/observable.html).

Article containing examples in relation to home automation: [Article with examples](https://dev.to/devjaspernl/supercharging-home-assistant-automations-initial-states-and-boolean-logic-for-netdaemon-rx-3bd5).

## Logical Operators

This library has extension methods for logical operators:

### Not

![Not](docs/img/Not.png)

### And

![And](docs/img/And.png)

### And (not distinct)

![And (not distinct)](docs/img/And%20(not%20distinct).png)

### Or

![Or](docs/img/Or.png)

### Or (not distinct)

![Or (not distinct)](docs/img/Or%20(not%20distinct).png)

### XOr

![XOr](docs/img/XOr.png)

## Scheduling

This library also has extension methods for scheduling:

### TrueForAtLeast

Returns an observable that won't emit false for at least timespan after an initial "true" is emitted by source.

![TrueForAtLeast](docs/img/TrueForAtLeast.png)

**Example Use Case**

Turn on a light for at least 3 seconds after a button was pressed. If 3 seconds are passed, only keep it on if the button is still being pressed, but immediately turn if off if not.
```csharp
// buttonPressed is a IObservable<bool>
var buttonPressed = button.StateChanges().Select(s => s.State == "pressed");
buttonPressed
    .TrueForAtLeast(TimeSpan.FromSeconds(3), scheduler)
    .SubscribeTrueFalse(
        () => light.TurnOn(),
        () => light.TurnOff());
```

### PersistTrueFor

Returns an observable that stays true for a time span once the source observable turns back to false.

![PersistTrueFor](docs/img/PersistTrueFor.png)

**Example Use Case**

Keep a light on for 3 more seconds after last motion was detected.
```csharp
// motionDetected is a IObservable<bool>
var motionDetected = motionSensor.StateChanges().Select(s => s.State == "motion detected");
motionDetected
    .PersistTrueFor(TimeSpan.FromSeconds(3), scheduler)
    .SubscribeTrueFalse(
        () => light.TurnOn(),
        () => light.TurnOff());
```

### WhenTrueFor

Returns an observable that emits true once the source observable emits true for a minimum time span.

![WhenTrueFor](docs/img/WhenTrueFor.png)

**Example Use Case**

Send notification when washing machine power has been 0 for at least 1 minute.
```csharp
// washingMachineCurrentIsZero is a IObservable<bool>
var washingMachineCurrentIsZero = washingMachineCurrent.StateChanges().Select(s => s.State == 0);
washingMachineCurrentIsZero
    .WhenTrueFor(TimeSpan.FromMinutes(1), scheduler)
    .SubscribeTrue(() => notification.Send("Washing machine is done!"));
```

### LimitTrueDuration

Returns an observable that stays true for a maximum of time span. If the source observable emits false before the time has passed, the resulting observable also emits false.

![LimitTrueDuration](docs/img/LimitTrueDuration.png)

**Example Use Case**

Keep closet lights on for a maximum amount of time.
```csharp
// closetDoorOpen is a IObservable<bool>
var closetDoorOpen = closetDoor.StateChanges().Select(s => s.State == "open");
closetDoorOpen
    .LimitTrueDuration(TimeSpan.FromMinutes(2), scheduler)
    .SubscribeTrueFalse(
        () => closetLight.TurnOn(),
        () => closetLight.TurnOff());
```

## Subscribing

Besides transformations, this library has extension methods that help with common cases of subscribing to implementations of `IObservable<bool>`: `SubscribeTrueFalse`, `SubscribeFalse` and `SubscribeTrue`.

### Example

```cs
boolObservable.SubscribeTrueFalse(
    () => {
        // Logic for when observable emits true.
    },
    () => {
        // Logic for when observable emits false.
    }
)
```