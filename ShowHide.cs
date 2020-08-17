using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowHide : MonoBehaviour
{
    //Utilized by the settings button to activate and de-activate teh settings panel
    public void TogglePanel()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
