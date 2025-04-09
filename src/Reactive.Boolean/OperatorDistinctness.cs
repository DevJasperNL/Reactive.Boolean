namespace Reactive.Boolean;

/// <summary>
/// Specifies the distinctness behavior of Boolean operators.
/// </summary>
public enum OperatorDistinctness
{
    /// <summary>
    /// Ensures that the output value is only emitted when the value has changed 
    /// compared to the previous emitted output value.
    /// </summary>
    OutputDistinctUntilChanged,

    /// <summary>
    /// Ensures that input values are only processed when they differ from the previous 
    /// input value.
    /// </summary>
    InputDistinctUntilChanged,

    /// <summary>
    /// No distinctness is applied. All values, including consecutive duplicates, 
    /// are emitted without filtering.
    /// </summary>
    NotDistinct
}