using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource shopMusic;
    public AudioSource unpackMusic;
    public AudioSource summaryMusic;
    public AudioSource endOfDayMusic;

    private float timeToFade = 1.0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void FadeToShopMusic()
    {
        float newSongVolume = 0.0f;
        float oldSongVolume = 0.5f;

        DOTween.To(() => newSongVolume, x => newSongVolume = x, 0.5f, this.timeToFade).OnUpdate(() =>
        {
            this.shopMusic.volume = newSongVolume;
        });

        DOTween.To(() => oldSongVolume, x => oldSongVolume = x, 0.0f, this.timeToFade).OnUpdate(() =>
        {
            this.endOfDayMusic.volume = oldSongVolume;
        });
    }

    public void FadeToPackMusic()
    {
        float newSongVolume = 0.0f;
        float oldSongVolume = 0.5f;

        DOTween.To(() => newSongVolume, x => newSongVolume = x, 0.5f, this.timeToFade).OnUpdate(() =>
        {
            this.unpackMusic.volume = newSongVolume;
        });

        DOTween.To(() => oldSongVolume, x => oldSongVolume = x, 0.0f, this.timeToFade).OnUpdate(() =>
        {
            this.shopMusic.volume = oldSongVolume;
        });
    }

    public void FadeToSummaryMusic()
    {
        float newSongVolume = 0.0f;
        float oldSongVolume = 0.5f;

        DOTween.To(() => newSongVolume, x => newSongVolume = x, 0.5f, this.timeToFade).OnUpdate(() =>
        {
            this.summaryMusic.volume = newSongVolume;
        });

        DOTween.To(() => oldSongVolume, x => oldSongVolume = x, 0.0f, this.timeToFade).OnUpdate(() =>
        {
            this.unpackMusic.volume = oldSongVolume;
        });
    }

    public void FadeToEndOfDayMusic()
    {
        float newSongVolume = 0.0f;
        float oldSongVolume = 0.5f;

        DOTween.To(() => newSongVolume, x => newSongVolume = x, 0.5f, this.timeToFade).OnUpdate(() =>
        {
            this.endOfDayMusic.volume = newSongVolume;
        });

        DOTween.To(() => oldSongVolume, x => oldSongVolume = x, 0.0f, this.timeToFade).OnUpdate(() =>
        {
            this.summaryMusic.volume = oldSongVolume;
        });
    }
}
