# Reactive.Boolean

Reactive Extensions meant specifically for implementations of `IObservable<bool>`

This documentation uses marble diagrams to explain the transformations of `IObservable<bool>`. More on marble diagrams can be found in the documentation of [ReactiveX](https://reactivex.io/documentation/observable.html).

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

### PersistTrueFor

Returns an observable that stays true for a time span once the base observable turns back to false.

![PersistTrueFor](docs/img/PersistTrueFor.png)

### WhenTrueFor

Returns an observable that emits true once the base observable emits true for a minimum time span.

![WhenTrueFor](docs/img/WhenTrueFor.png)

### LimitTrueDuration

Returns an observable that stays true for a maximum of time span. If the base observable emits false before the time has passed, the resulting observable also emits false.

![LimitTrueDuration](docs/img/LimitTrueDuration.png)

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