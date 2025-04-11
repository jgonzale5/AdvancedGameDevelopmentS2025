using System.Collections.Generic;
using UnityEngine;

public class VoiceDialogueManager : MonoBehaviour
{
    //The file with the dialogue in each of the languages
    public TextAsset csvLanguageFile;
    //The dictionary where we'll load the dialogue
    private Dictionary<string, List<string>> languageDict = new Dictionary<string, List<string>>();
    //The index corresponding to each of the language initials
    private Dictionary<string, int> languageIndexes = new Dictionary<string, int>();
    //The initials of the currently selected language
    public string languageSelected = "EN";

    /// <summary>
    /// The voice clips for the game
    /// </summary>
    public VoiceClipSO[] voiceClips;
    /// <summary>
    /// A dictionary to quickly find and access the voice clips
    /// </summary>
    private Dictionary<string, VoiceClipSO> voiceClipDict = new Dictionary<string, VoiceClipSO>();

    //A singleton of this script so we can reference it later
    public static VoiceDialogueManager Instance;

    public void Init()
    {
        //Load the file
        LoadFile();

        ///Build the dictionary from the array of voice clips
        foreach (var clip in voiceClips)
        {
            voiceClipDict.Add(clip.textCode, clip);
        }

        //Set the instance
        Instance = this;
    }

    //This function loads the CSV file into the dictionary
    private void LoadFile()
    {
        //We load the text asset into a string
        string fileContents = csvLanguageFile.text;

        //We split the string into the lines that make up the file
        string[] lines = fileContents.Split("\n");

        //We split the first line into its cells
        //We'll use this variable to repeat the process with the other lines
        string[] cells = lines[0].Split(",");

        //We load each name into the languageIndexDictionary
        for (int n = 1; n < cells.Length; n++)
        {
            //MAKE SURE TO DO N-1, OTHERWISE IT'LL BE SHIFTED ONE TO THE RIGHT
            languageIndexes.Add(cells[n], n - 1);
        }

        //We are going line by line, skipping the first line
        for (int l = 1; l < lines.Length; l++)
        {
            //We split the line into the cells that make it
            cells = lines[l].Split(",");

            //We make a list to store it
            List<string> languages = new List<string>();

            //For each cell in the line, skippin the first one (the ID)
            for (int c = 1; c < cells.Length; c++)
            {
                //We add it ot the list
                languages.Add(cells[c]);
            }

            //We register the languages in the dictionary
            languageDict.Add(cells[0], languages);
        }
    }

    /// <summary>
    /// Returns the line corresponding to the specified key
    /// </summary>
    /// <param name="key">The key for the dialogue line</param>
    /// <returns>Returns the string for that key in the selected language</returns>
    public string GetLine(string key)
    {
        //If the languahe dictionary contains the key received
        if (languageDict.ContainsKey(key))
        {
            //We find the index related to that language
            int langSel = languageIndexes[languageSelected];

            //Return the line corresponding to that key and language
            return languageDict[key][langSel];
        }

        return "ERROR: Line not found";
    }

    /// <summary>
    /// Returns the audioclip associated with the given key
    /// </summary>
    /// <param name="key">The key that will get us the voice clip for the line given</param>
    /// <returns>Returns the clip for that key in the selected language.</returns>
    public AudioClip GetVoiceLine(string key)
    {
        if (voiceClipDict.ContainsKey(key))
        {
            return voiceClipDict[key].GetClip(languageSelected);
        }

        return null;
    }
}
