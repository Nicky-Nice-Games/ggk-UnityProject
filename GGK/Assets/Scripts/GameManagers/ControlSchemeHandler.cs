using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlSchemeHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset actions;

    InputAction accelerate;

    private void Awake()
    {
        // get action map
        InputActionMap carControls = actions.FindActionMap("Car");

        // get specific action
        accelerate = carControls.FindAction("Accelerate");

        accelerate.Enable();
    }


    /// <summary>
    /// changes controller bindings when player chooses different scheme
    /// </summary>
    /// <param name="index">players preferred scheme</param>
    public void OnSchemeChange(int index)
    {
        accelerate.actionMap.Disable();

        switch (index)
        {
            case 0:
                ResetBindings();
                break;
            case 1:
                RemapToTriggers();
                break;
            default:
                break;
        }

        accelerate.actionMap.Enable();
    }

    /// <summary>
    /// changes controller accelerate bindings to triggers
    /// </summary>
    private void RemapToTriggers()
    {
        for (int i = 0; i < accelerate.bindings.Count; i++)
        {
            InputBinding binding = accelerate.bindings[i];

            // only care about child bindings
            if (!binding.isPartOfComposite) continue;

            if (binding.name.ToLowerInvariant() == "negative") // decelerate goes to left trigger
            {
                accelerate.ApplyBindingOverride(i, "<Gamepad>/leftTrigger");
            }
            else if (binding.name.ToLowerInvariant() == "positive") // accelerate goes to right trigger
            {
                accelerate.ApplyBindingOverride(i, "<Gamepad>/rightTrigger");
            }
        }
    }

    /// <summary>
    /// resets to original bindings
    /// </summary>
    private void ResetBindings()
    {
        accelerate.RemoveAllBindingOverrides();
    }
}
