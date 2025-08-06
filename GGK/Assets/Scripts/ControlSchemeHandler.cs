using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlSchemeHandler : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset actions;

    InputAction accelerate;

    private TMP_Dropdown presetDropdown;

    private bool LTNegative = false;
    private bool RTPositive = false;

    private void Awake()
    {
        // get action map
        InputActionMap carControls = actions.FindActionMap("Car");

        // get specific action
        accelerate = carControls.FindAction("Accelerate");

        accelerate.Enable();
    }

    private void OnEnable()
    {
        // get a reference to the dropdown
        presetDropdown = GetComponent<TMP_Dropdown>();
    }

    private void Update()
    {
        if (presetDropdown != null)
        {
            // checks if the accelerate and decelerate bindings are binded to the triggers
            for (int i = 0; i < accelerate.bindings.Count; i++)
            {
                InputBinding binding = accelerate.bindings[i];

                if (binding.effectivePath == "<Gamepad>/leftTrigger" && binding.name.ToLowerInvariant() == "negative")
                {
                    LTNegative = true;
                }
                else if (binding.effectivePath != "<Gamepad>/leftTrigger" && binding.name.ToLowerInvariant() == "negative")
                {
                    LTNegative = false;
                }

                if (binding.effectivePath == "<Gamepad>/rightTrigger" && binding.name.ToLowerInvariant() == "positive")
                {
                    RTPositive = true;
                }
                else if (binding.effectivePath != "<Gamepad>/rightTrigger" && binding.name.ToLowerInvariant() == "positive")
                {
                    RTPositive = false;
                }
            }

            // displays preset dropdown across screens and makes sure right preset is corresponding with controls
            if (LTNegative && RTPositive)
            {
                presetDropdown.value = 1;
            }
            else
            {
                presetDropdown.value = 0;
            }
        }
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
