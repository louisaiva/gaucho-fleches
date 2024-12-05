using UnityEngine;

public class SaveBeforeQuitWindow : MonoBehaviour
{
    public GameObject window;

    public void Show()
    {
        window.SetActive(true);
    }

    public void Hide()
    {
        window.SetActive(false);
    }
}
