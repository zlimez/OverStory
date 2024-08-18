using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// Represents a choice in a decision-making system.
/// </summary>
public class Choice
{
    /// <summary>
    /// The prefix used for choice related events.
    /// </summary>
    public static string EventPrefix = "Hector Chose: ";

    /// <summary>
    /// The text associated with this choice.
    /// </summary>
    public string ChoiceText { get; private set; }

    /// <summary>
    /// Indicates if the choice is currently activated.
    /// </summary>
    public bool IsActivated;

    /// <summary>
    /// The event that is triggered when this choice is selected.
    /// </summary>
    public UnityAction<object> ChoiceSelectedEvent { get; private set; }

    /// <summary>
    /// Creates a new Choice instance with the specified text, activation status, and event.
    /// </summary>
    /// <param name="choiceText">The text associated with the choice.</param>
    /// <param name="choiceSelectedEvent">The event that is triggered when the choice is selected.</param>
    /// <param name="isActivated">Indicates if the choice is currently activated.</param>
    public Choice(string choiceText, UnityAction<object> choiceSelectedEvent, bool isActivated = true)
    {
        ChoiceText = choiceText;
        ChoiceSelectedEvent = choiceSelectedEvent;
        IsActivated = isActivated;
    }
}