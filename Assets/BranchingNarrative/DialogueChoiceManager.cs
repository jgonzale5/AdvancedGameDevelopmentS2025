using UnityEngine;
//Add this for the unity events
using UnityEngine.Events;
//A reference to the ink runtime library to get access to choices
using Ink.Runtime;
//We add this to use lists
using System.Collections.Generic;
using UnityEngine.UIElements;

public class DialogueChoiceManager : MonoBehaviour
{
    //A reference to the dialogue manager
    public DialogueManager dialogueManager;

    //What happens when a choice becomes available
    public UnityEvent OnChoiceAvailable;
    //What happens when a line comes in and a choice is not available
    public UnityEvent OnChoiceNotAvailable;
    //What happens when a choice is selected
    public UnityEvent OnChoiceSelected;

    //A reference to the buttons so we can enable only the ones we need
    public List<GameObject> choiceButtons = new List<GameObject>();

    //This is called by the buttons so we can do things like make sounds when a choice is selected
    public void SelectChoice(int index)
    {
        //Tell the dialogue manager what choice was made
        dialogueManager.MakeChoice(index);
        //Invoke the events that should happen when a choice is made
        OnChoiceSelected.Invoke();
    }

    //This function is called to spawn the options
    public void ChoicesAvailable(List<Choice> choices)
    {
        //Hide all the buttons
        foreach (GameObject g in choiceButtons)
        {
            g.SetActive(false);
        }

        //If there are no choices...
        if (choices.Count == 0)
        {
            //Invoke the corresponding event
            OnChoiceNotAvailable.Invoke();
            //End the function
            return;
        }

        //If there are choices...
        //Go through each of them
        for (int i = 0; i < choices.Count; i++)
        {
            //Spawn a button for each
            choiceButtons[i].SetActive(true);
        }
        
        //Invoke the corresponding event
        OnChoiceAvailable.Invoke();
    }
}
