using UnityEngine;

[CreateAssetMenu(fileName = "VoiceClipSO", menuName = "Scriptable Objects/VoiceClipSO")]
public class VoiceClipSO : ScriptableObject
{
    ///The line code associated with this clip e.g. intro_1 
    public string textCode;

    /// <summary>
    /// A class to associate each clip with a specific language
    /// </summary>
    [System.Serializable]
    public class VoiceClip
    {
        public string languageInitials;
        public AudioClip voiceClip;
    }

    /// <summary>
    /// An array of all the clips for each different language
    /// </summary>
    public VoiceClip[] voiceClips;

    public AudioClip GetClip(string language)
    {
        foreach (var clip in voiceClips)
        {
            if (clip.languageInitials == language)
            {
                return clip.voiceClip;
            }
        }

        return null;
    }


}
