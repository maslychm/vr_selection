using UnityEngine.Events;

// Enables passing a string through an event
public class FilterSelectionEvent : UnityEvent<string>
{ }

public class SelectionEvents : Singleton<SelectionEvents>
{
    public static FilterSelectionEvent FilterSelection = new FilterSelectionEvent();
}