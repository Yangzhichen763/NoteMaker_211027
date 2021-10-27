using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public enum AudioType
    {
        Bubble,
    }

    public static AudioPlayer player;
    [SerializeField] private AudioClip[] bubbles;

    public void Awake()
    {
        if (player == null)
        {
            player = this;
            DontDestroyOnLoad(this);
        }
        else 
        {
            this.enabled = false;
            return;
        }
    }

    public AudioClip GetRandomClip(AudioType audioType)
    {
        AudioClip[] clipGroup;
        switch (audioType)
        {
            case AudioType.Bubble:
                clipGroup = bubbles;
                break;
            default:
                clipGroup = bubbles;
                break;
        }

        int randIndex = Random.Range(0, clipGroup.Length);
        return clipGroup[randIndex];
    }
}
