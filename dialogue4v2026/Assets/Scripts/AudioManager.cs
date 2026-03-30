using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    // Índice selecionado globalmente (pode ser alterado via editor custom ou por scripts)
    public int SelectedIndex = 0;

    private AudioSource systemSource;
    private List<AudioSource> activeSources;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            systemSource = gameObject.GetComponent<AudioSource>();
            activeSources = new List<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Funções de gerenciamento de audio 2D
    public void PlaySound(AudioClip clip)
    {
        systemSource.Stop();
        systemSource.clip = clip;
        systemSource.Play();
    }

    public void StopSound()
    {
        systemSource.Stop();
    }

    public void PauseSound()
    {
        systemSource.Pause();
    }

    public void ResumeSound()
    {
        systemSource.UnPause();
    }
    
    public void PlayOneShot(AudioClip clip)
    {
        systemSource.PlayOneShot(clip);
    }

    // Play a clip from an AudioCollection by index (2D/system source)
    public void PlayCollection(AudioCollection collection, int index)
    {
        if (collection == null || collection.AudioClipCollection == null) return;
        if (index < 0 || index >= collection.AudioClipCollection.Count) return;
        PlaySound(collection.AudioClipCollection[index]);
    }

    // Play using the current SelectedIndex
    public void PlayCollection(AudioCollection collection)
    {
        PlayCollection(collection, SelectedIndex);
    }

    public void SetSelectedIndex(int index)
    {
        SelectedIndex = index;
    }
    
    // Funções de gerenciamento de audio 3d
    public void PlaySound(AudioClip clip, AudioSource source)
    {
        if(!activeSources.Contains(source)) activeSources.Add(source);
        source.Stop();
        source.clip = clip;
        source.Play();
    }

    public void StopSound(AudioSource source)
    {
        source.Stop();
        activeSources.Remove(source);
    }

    public void PauseSound(AudioSource source)
    {
        source.Pause();
    }
    
    public void ResumeSound(AudioSource source)
    {
        source.UnPause();
    }

    public void PlayOneShot(AudioClip clip, AudioSource source)
    {
        source.PlayOneShot(clip);
    }
}
