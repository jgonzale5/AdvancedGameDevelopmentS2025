using UnityEngine;
//We add the TMPro library to be able to use it
using TMPro;

public class SubtitleManager : MonoBehaviour
{
    /// <summary>
    /// A reference to the manager that loads the CSV file and the voice clip SOs
    /// </summary>
    public VoiceDialogueManager voiceManager;
    /// <summary>
    /// The text on screen that will display the subtitles
    /// </summary>
    public TextMeshProUGUI subtitle;
    /// <summary>
    /// The source that will play the voice lines
    /// </summary>
    public AudioSource audioSource;

    /// <summary>
    /// Initialize the init function
    /// </summary>
    private void Start()
    {
        if (VoiceDialogueManager.Instance == null)
            voiceManager.Init();
    }

    /// <summary>
    /// Plays a given dialogue line associated with the passed key
    /// </summary>
    /// <param name="key">The key used to get the line and the audioclip</param>
    public void PlayDialogue(string key)
    {
        //Set the text of the subtitle to the corresponding line
        subtitle.text = voiceManager.GetLine(key);

        //Get the voice clip associated with the given line
        AudioClip clip = voiceManager.GetVoiceLine(key);

        //If the clip isn't null, play it
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    /// <summary>
    /// Clears the subtitle so we can hide the text after the line has been said
    /// </summary>
    public void ClearSubtitle()
    {
        subtitle.text = string.Empty;
    }
}
