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
    //A reference to the script that handles the choice buttons
    public DialogueChoiceManager dialogueChoiceManager;

    void Start()
    {
        //We initialize the story with the text
        activeScene = new Story(scene.text);

        if (DialogueLanguageLoader.Instance == null)
        {
            FindFirstObjectByType<DialogueLanguageLoader>().Init();
        }

        //We debug what it says 
        //Debug.Log(activeScene.Continue());

        //Display the next line in the dialogue box 
        //dialogueBoxManager.DisplayText(activeScene.Continue());
        Next();
    }

    //This function progresses the story and updates the dialogue box with the next line
    public void Next()
    {
        //If the active scene isn't at the end
        if (activeScene.canContinue)
        {
            //Cache the line
            string line = activeScene.Continue();

            //If there are tags on this line
            if (activeScene.currentTags.Count > 0)
            {
                //Use the language selected
                line = DialogueLanguageLoader.Instance.GetLine(activeScene.currentTags[0]);
            }

            //If we have reached a point with a decision
            if (activeScene.currentChoices.Count > 0)
            {
                //Initialize an index so we can display the options
                int choiceIndex = 0;

                //For each choice
                foreach (var choice in activeScene.currentChoices)
                {
                    string choiceText = activeScene.currentChoices[choiceIndex].text;
                    
                    //If the choice is tagged with a line ID
                    if (choice.tags.Count > 0)
                    {
                        //Get the corresponding line and set it to be the choice text
                        choiceText = DialogueLanguageLoader.Instance.GetLine(choice.tags[0]);
                    }

                    //Add a new line showin what button to press
                    line += "\n" + (choiceIndex + 1).ToString() + ": " + choiceText;
                    //Increase the index by 1
                    choiceIndex++;
                }
            }

            //Let the choice manager handle the choices presented
            dialogueChoiceManager.ChoicesAvailable(activeScene.currentChoices);

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
