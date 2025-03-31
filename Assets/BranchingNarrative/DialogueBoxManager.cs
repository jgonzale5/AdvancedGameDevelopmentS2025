using UnityEngine;
//We add the TMPro library to use the text mesh pro text
using TMPro;

public class DialogueBoxManager : MonoBehaviour
{
    //The text that displays the dialogue
    public TextMeshProUGUI dialogueText;

    //Change the text on the dialogue box to match the passed argument
    public void DisplayText(string text)
    {
        this.dialogueText.text = text;
    }
}
