using UnityEngine;
//We add the Ink API
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    //This is the ink file we'll be loading
    public TextAsset scene;

    //The scene currently active. This keeps track of decisions and such.
    private Story activeScene;

    //The script that handles the display of information on the dialogue box
    public DialogueBoxManager dialogueBoxManager;

    void Start()
    {
        //We initialize the story with the text
        activeScene = new Story(scene.text);

        //We debug what it says 
        //Debug.Log(activeScene.Continue());

        //Display the next line in the dialogue box 
        dialogueBoxManager.DisplayText(activeScene.Continue());
    }

    //This function progresses the story and updates the dialogue box with the next line
    public void Next()
    {
        //If the active scene isn't at the end
        if (activeScene.canContinue)
        {
            //Cache the line
            string line = activeScene.Continue();

            //If we have reached a point with a decision
            if (activeScene.currentChoices.Count > 0)
            {
                //Initialize an index so we can display the options
                int choiceIndex = 0;

                //For each choice
                foreach (var choice in activeScene.currentChoices)
                {
                    //Add a new line showin what button to press
                    line += "\n" + (choiceIndex + 1).ToString() + ": " + activeScene.currentChoices[choiceIndex].text;
                    //Increase the index by 1
                    choiceIndex++;
                }
            }

            //progress
            dialogueBoxManager.DisplayText(line);
        } 
    }

    //This function is called to make a decision
    public void MakeChoice(int index)
    {
        //Tell the story we are picking the choice of the index passed
        activeScene.ChooseChoiceIndex(index);

        //Move to the next scnee
        Next();
    }
}
