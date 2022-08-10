using UnityEngine.Events;

// Enables passing a string through an event
public class FilterSelectionEvent : UnityEvent<RecognitionResult>
{ }

public class DirectionSelectionEvent : UnityEvent<RecognitionResult>
{ }

public class SelectionEvents : Singleton<SelectionEvents>
{
    public static FilterSelectionEvent FilterSelection = new FilterSelectionEvent();
    public static DirectionSelectionEvent DirectionSelection = new DirectionSelectionEvent();
}