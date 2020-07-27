using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

// ********** [[ DOTween Required ]] **********

public class MusicManager : MonoBehaviour
{
    AudioSource BGM, SFX, toBeDeleted;
    AudioMixerGroup MasterMixer, BGMMixer, SFXMixer;
    AudioMixer audioMixer;
    GameSystem gameSystem;
    UserSetting setting;


    void Awake()
    {
        gameSystem = GameObject.FindObjectOfType<GameSystem>();

        setting = gameSystem.LoadUserSetting();
        MasterMixer = gameSystem.MasterMixer;
        BGMMixer = gameSystem.BGMMixer;
        SFXMixer = gameSystem.SFXMixer;

        audioMixer = MasterMixer.audioMixer;

        // Get BGM Object
        if (GameObject.Find("BGM") != null)
            BGM = GameObject.Find("BGM").GetComponent<AudioSource>();
        else
        {
            BGM = gameObject.AddComponent<AudioSource>();
            Initialize(BGM, 0, true);
            BGM.outputAudioMixerGroup = BGMMixer;
        }


        // Get SFX Object
        if (GameObject.Find("SFX") != null)
            SFX = GameObject.Find("SFX").GetComponent<AudioSource>();
        else
        {
            SFX = gameObject.AddComponent<AudioSource>();
            Initialize(SFX, 1, false);
            SFX.outputAudioMixerGroup = SFXMixer;
        }

    }

    void Start()
    {
        if (setting == null)
            setting = gameSystem.LoadUserSetting();

        if (setting.isBGMMute)
            BGM.mute = true;
        else
        {
            BGM.mute = false;
            BGM.volume = setting.BGMVol;
            //audioMixer.SetFloat("BGMVol", Mathf.Log(gameData.setting.BGMVol) * 20);
        }

        if (setting.isSFXMute)
            SFX.mute = true;
        else
        {
            SFX.mute = false;
            SFX.volume = setting.SFXVol;
            //audioMixer.SetFloat("SFXVol", Mathf.Log(gameData.setting.SFXVol) * 20);
        }
    }

    void Initialize(AudioSource audioSource, float initialVol, bool isLoop)
    {
        audioSource.volume = initialVol;
        audioSource.loop = isLoop;

        // Default is false
        audioSource.playOnAwake = false;
        //audioSource.outputAudioMixerGroup = audioMixerGroup;
    }

    /*
        public void PlayMusic()
        {
            if (!BGM.isPlaying || (BGM.isPlaying && !musicMode))
            {
                musicMode = true;
                BGM.loop = false;
                currentID = 0;
                musicID = new int[playlist.music.Length];
                for (int i = 0; i < playlist.music.Length; i++)
                {
                    musicID[i] = i;
                }
                musicID = Shuffle(musicID);
                PlayBGM(playlist.music[musicID[currentID]], 1, 3);
                if (musicInfo == null) musicInfo = GameObject.FindObjectOfType<MusicInfo>();
                musicInfo.DisplayInfo(playlist.artist[musicID[currentID]], playlist.song[musicID[currentID]]);
            }
        }

        public void DisableMusicMode()
        {
            musicMode = false;
            BGM.loop = true;
        }
    */
    public void PlayBGM(AudioClip BGMClip, float targetVol, float fadeInTime)
    {
        if (BGM.isPlaying)
        {
            StartCoroutine(CrossFadeBGM(BGMClip, targetVol, fadeInTime));
        }
        else
        {
            BGM.clip = BGMClip;
            BGM.Play();
            BGM.DOFade(targetVol, fadeInTime);
        }
    }

    public void StopBGM(float fadeOutTime)
    {
        if (fadeOutTime == 0)
            BGM.Stop();
        else
            BGM.DOFade(0, fadeOutTime);
    }

    public void PauseBGM()
    {
        BGM.Pause();
    }

    public IEnumerator CrossFadeBGM(AudioClip BGMClip, float targetVol, float fadeInTime)
    {
        toBeDeleted = BGM;
        BGM = gameObject.AddComponent<AudioSource>();
        // if (musicMode)
        //     Initialize(BGM, 0, false);
        // else
        Initialize(BGM, 0, true);
        BGM.outputAudioMixerGroup = BGMMixer;
        toBeDeleted.DOFade(0, fadeInTime);
        PlayBGM(BGMClip, targetVol, fadeInTime);
        yield return new WaitForSeconds(fadeInTime);
        Destroy(toBeDeleted);
    }

    public void FadeBGM(float targetVol, float fadeTime)
    {
        BGM.DOFade(targetVol, fadeTime);
    }

    public void PlaySFX(AudioClip SFXClip)
    {
        SFX.PlayOneShot(SFXClip);
    }

    public void PlaySFX(AudioClip SFXClip, float targetVol)
    {
        SFX.PlayOneShot(SFXClip, targetVol);
    }

    // public void PlaySFX(AudioClip SFXClip, float targetVol, bool isRandomPitch){
    //     SFX.pitch = Random.Range(0.8f,1.2f);
    //     SFX.PlayOneShot(SFXClip,targetVol);
    //     Invoke("RecoverPitch",0.1f);
    // }

    // void RecoverPitch(){
    //     SFX.pitch = 1;
    // }

    public void ChangeSnapshot(string snapshot)
    {
        AudioMixerSnapshot audioMixerSnapshot = MasterMixer.audioMixer.FindSnapshot(snapshot);
        audioMixerSnapshot.TransitionTo(0.2f);
    }

    public void ChangeSnapshot(string snapshot, float transitionTime)
    {
        AudioMixerSnapshot audioMixerSnapshot = MasterMixer.audioMixer.FindSnapshot(snapshot);
        audioMixerSnapshot.TransitionTo(transitionTime);
    }


    public void DoBGMPitch(float targetPitch, float targetTime)
    {
        BGM.DOPitch(targetPitch, targetTime);
    }

    public bool MuteBGM()
    {
        BGM.mute = !BGM.mute;
        return BGM.mute;
    }

    public bool MuteSFX()
    {
        SFX.mute = !SFX.mute;
        return SFX.mute;
    }

    public void MuteAll()
    {
        BGM.mute = true;
        SFX.mute = true;
    }

    public void UnmuteAll()
    {
        BGM.mute = false;
        SFX.mute = false;
    }

    public void ChangeVol(int type, float vol)
    {
        switch (type)
        {
            case 0:
                BGM.volume = vol;
                // audioMixer.SetFloat("BGMVol", Mathf.Log(vol) * 20);
                break;
            case 1:
                SFX.volume = vol;
                // audioMixer.SetFloat("SFXVol", Mathf.Log(vol) * 20);
                break;
            default:
                break;
        }
    }

    int[] Shuffle(int[] list)
    {
        int n = list.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            int id = list[k];
            list[k] = list[n];
            list[n] = id;
        }
        return list;
    }
}
