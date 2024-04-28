using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class cinematicDialogues : MonoBehaviour
{
    [SerializeField] string[] sentences;
    int index;
    bool isOndial, canDial;
    public float increaseTime = 3f; // Temps entre chaque augmentation (en secondes)
    private float timer = 0f;
    
    HUDManager manager => HUDManager.instance;

    private void Update()
    {
        if (canDial)
        {
            timer += Time.deltaTime;
            StartDialogue();
            if (timer >= increaseTime)
            {
                NextLine();
                timer = 0f;
            }
        }
    }

    public void StartDialogue()
    {
        manager.dialogueHolder.SetActive(true);
        isOndial = true;
        TypingText(sentences);
    }

    void TypingText(string[] sentence)
    {
        manager.textDisplay.text = "";
        manager.textDisplay.text = sentence[index];
        if (manager.textDisplay.text == sentence[index])
            manager.continueButton.SetActive(true);
    }

    public void NextLine()
    {
        manager.continueButton.SetActive(false);
        if (isOndial && index < sentences.Length - 1)
        {
            index++;
            manager.textDisplay.text = "";
            TypingText(sentences);
        }
        else if (isOndial && index == sentences.Length - 1)
        {
            isOndial = false;
            index = 0;
            manager.textDisplay.text = "";
            manager.dialogueHolder.SetActive(false);

            // Désactiver le dialogue lorsque toutes les phrases ont été affichées
            canDial = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "PlayerA" || col.tag == "PlayerB")
        {
            canDial = true;
        }
    }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.tag == "PlayerA" || other.tag == "PlayerB")
    //         canDial = false;
    // }
}