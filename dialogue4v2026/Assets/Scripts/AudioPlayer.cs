using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioCollection myAudioCollection;
    [Tooltip("Índice do clip dentro da AudioCollection que será usado para Play quando solicitado")]
    public int clipIndex = 0;

    [Tooltip("Se verdadeiro, tentará aplicar o clipIndex ao AudioManager.SelectedIndex quando Play for acionado (apenas em Play Mode)")]
    public bool applyIndexToManagerOnPlay = true;

    // no Start() - avoid auto-play on scene load

    // Play the configured clip (uses AudioManager singleton). Safe if manager or collection is null.
    public void Play()
    {
        if (myAudioCollection == null)
        {
            Debug.LogWarning("AudioPlayer: no AudioCollection assigned.");
            return;
        }

        if (applyIndexToManagerOnPlay && AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSelectedIndex(clipIndex);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCollection(myAudioCollection, clipIndex);
        }
        else
        {
            Debug.LogWarning("AudioPlayer: AudioManager instance not found. Are you in Play Mode and is AudioManager present in the scene?");
        }
    }

    public void Stop()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopSound();
        }
    }

    public void Pause()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PauseSound();
        }
    }

    public void Resume()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ResumeSound();
        }
    }

    // Apply the clipIndex to the runtime AudioManager singleton SelectedIndex (only works in Play Mode)
    public void ApplyIndexToManager()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSelectedIndex(clipIndex);
        }
        else
        {
            Debug.LogWarning("AudioPlayer: AudioManager instance not found. Cannot apply index outside Play Mode.");
        }
    }
}
