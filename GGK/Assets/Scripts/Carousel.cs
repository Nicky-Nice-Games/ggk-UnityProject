using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Carousel : MonoBehaviour
{
    [SerializeField] private List<Image> buttons;

    //how many out of the button list are visible on the carousel at a time?
    //preferrably less than the total amount of buttons
    [SerializeField] private int carouselAmount;

    //minimum and maximum size of buttons--button size scales with displacement from middle
    [SerializeField] private Vector2 minSize;
    [SerializeField] private Vector2 maxSize;

    //how opaque is the farthest button away from the middle?
    [SerializeField] private float minOpacity;

    //how far does the Axis reading have to be before an input is registered?
    [SerializeField] private float sensitivity;

    //width and height values of the carousel
    [SerializeField] private float width;
    [SerializeField] private float height;

    //how much should the timer be set to after an input reading?
    [SerializeField] private float debounceTimerMax;

    //debounce essentially prevents multiple inputs from one press/flick
    //until the player is ready for another input 
    private bool debounce;
    private float debounceTimer;

    // Start is called before the first frame update
    void Start()
    {
        debounce = false;
        RefreshImages();
    }

    // Update is called once per frame
    void Update()
    {
        float hInput;
        //if there are any gamepads..
        if (Gamepad.all.Count > 0)
        {
            //..use GetAxis
            hInput = Input.GetAxis("Horizontal");
        }
        else
        {
            //if it's a controller, use Raw (to count every input as valid)
            hInput = Input.GetAxisRaw("Horizontal");
        }

        //if an input is registered..
        if (!debounce && Mathf.Abs(hInput) >= sensitivity)
        {
            //forward
            if (hInput < 0)
            {
                //this puts the first item in the back, shuffling all the items up by one
                Image b = buttons[0];
                buttons.RemoveAt(0);
                buttons.Add(b);
            }
            //back
            else if (hInput > 0)
            {
                //this puts the last item in the front, shuffling all the items down by one
                Image b = buttons[buttons.Count - 1];
                buttons.RemoveAt(buttons.Count - 1);
                buttons.Insert(0, b);
            }

            //reshuffle the images around
            debounce = true;
            debounceTimer = debounceTimerMax;
            RefreshImages();
        }

        //tick down/reset the debounce timer
        if (debounceTimer > 0)
        {
            debounceTimer -= Time.deltaTime;
        }
        else if (debounceTimer <= 0 && debounce == true)
        {
            debounce = false;
        }
    }

    private void RefreshImages()
    {
        //for all images in the image list..
        for (int i = 0; i < buttons.Count; i++)
        {
            Image b = buttons[i];

            //if the image is included in the carousel..
            if (i < (carouselAmount))
            {
                //calculate its index in carousel (index from the middle)
                int carPos = IndexToCaroselPos(i);
                //calculate its displacement in carousel (index but supports negative numbers
                //aka distingusihes between left and right of the middle)
                int dis = IndexToDisplacement(i);

                //rearrange things so things closer to the center appear over things farther away
                b.transform.SetSiblingIndex(b.transform.parent.childCount - (carPos + 1));
                //calculate percentage of distance from center
                float lerp = (float)carPos / (float)(carouselAmount / 2);

                //calculate opacity based on minimum opacity and distance from center
                b.color = new Color(b.color.r, b.color.g, b.color.b, Mathf.Lerp(minOpacity, 1, 1 - lerp));
                //calculate itsposition based on distance from center
                b.rectTransform.sizeDelta = Vector2.Lerp(maxSize, minSize, lerp);
                //calculate its position based on width and height
                b.rectTransform.localPosition = new Vector2((Mathf.Sin((float)dis / 2.0f) * width), (Mathf.Cos((float)dis / 2.0f) * -height));
            }
            else
            {
                //MAKE IT TINY!!! AND UH.. INVISBLE! YEAH.
                b.color = new Color(b.color.r, b.color.g, b.color.b, 0);
                b.rectTransform.sizeDelta = new Vector2(0, 0);
            }
        }
    }

    /// <summary>
    /// Calculates index from the center. 
    /// </summary>
    /// <param name="index">the index in the list the carousel is in.</param>
    /// <returns></returns>
    private int IndexToCaroselPos(int index)
    {
        return (int)Mathf.Abs(index - (carouselAmount / 2));
    }

    /// <summary>
    /// Calculates displacement from the center. 
    /// </summary>
    /// <param name="index">the index in the list the carousel is in.</param>
    /// <returns></returns>
    private int IndexToDisplacement(int index)
    {
        return (int)(index - (carouselAmount / 2));
    }

}
