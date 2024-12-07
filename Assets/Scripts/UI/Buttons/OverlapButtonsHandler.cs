using UnityEngine;
using UnityEngine.UI;

public class OverlapButtonsHandler : MonoBehaviour
{
    // simple script to defocus a button when another one is hovered
    public HoverButton otherButton;
    public Graphic graphic;

    bool disabled_the_other_button = false;

    void Update()
    {
        if (otherButton == null || graphic == null) { return; }

        // we check if the mouse if over the graphic
        bool mouse_over = RectTransformUtility.RectangleContainsScreenPoint(graphic.rectTransform, Input.mousePosition);

        // we check if the mouse if over the graphic
        if (mouse_over && !disabled_the_other_button)
        {
            Debug.Log("Mouse over the graphic");

            // we disable the other button
            otherButton.interactable = false;
            disabled_the_other_button = true;
        }
        else if (!mouse_over && disabled_the_other_button)
        {
            Debug.Log("Mouse not over the graphic");

            // we enable the other button
            otherButton.interactable = true;
            disabled_the_other_button = false;
        }
    }

}