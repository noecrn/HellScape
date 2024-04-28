using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExamineRef : MonoBehaviour
{
    public static ExamineRef instance;

    [Header("Examine Fields")]
    //Examine window object
    public GameObject examineWindow;
    public Image examineImage;
    public Text examineText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
