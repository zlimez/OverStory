using UnityEngine;

/// <summary>
/// Handles player input and sets button activation flags for other scripts to access.
/// </summary>
public class InputManager : MonoBehaviour
{
    [Header("Button Activation Flags")]
    /// <summary>
    /// Indicates if the interact button has been activated.
    /// </summary>
    public static bool InteractButtonActivated;

    /// <summary>
    /// Indicates if the dialog button has been activated.
    /// </summary>
    public static bool DialogButtonActivated;

    /// <summary>
    /// Indicates if the choice button has been activated.
    /// </summary>
    public static bool InventoryButtonActivated;

    private void Awake()
    {
        ResetStaticVariables();
    }

    private void Update()
    {
        HandleInput();
    }

    /// <summary>
    /// Resets the static variables to their default values.
    /// </summary>
    private void ResetStaticVariables()
    {
        InteractButtonActivated = false;
        DialogButtonActivated = false;
    }

    /// <summary>
    /// Handles player input and sets the appropriate button activation flags.
    /// </summary>
    private void HandleInput()
    {
        if (IsSubmitButtonPressed())
        {
            ProcessSubmitButton();
        }
        else
        {
            ResetButtonActivationFlags();
        }
    }

    /// <summary>
    /// Checks if the submit button has been pressed.
    /// </summary>
    /// <returns>True if the submit button has been pressed, otherwise false.</returns>
    private bool IsSubmitButtonPressed()
    {
        return Input.GetButtonDown("Submit") || Input.GetMouseButtonDown(0);
    }

    /// <summary>
    /// Processes the submit button press and sets the corresponding button activation flags.
    /// </summary>
    private void ProcessSubmitButton()
    {

        if (DialogueManager.Instance.InDialogue)
        {
            DialogButtonActivated = true;
            return;
        }

        if (!UiStatus.IsOpen)
        {
            InteractButtonActivated = true;
        }
    }

    /// <summary>
    /// Resets the button activation flags.
    /// </summary>
    private void ResetButtonActivationFlags()
    {
        DialogButtonActivated = false;
        InteractButtonActivated = false;
    }
}
