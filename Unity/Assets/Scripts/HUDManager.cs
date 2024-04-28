using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager instance;
    public GameObject dialogueHolder, continueButton;
    public TextMeshProUGUI  textDisplay;
    public Text TextOnTrigger;

    private void Awake()
    {
        instance = this;
    }
}
