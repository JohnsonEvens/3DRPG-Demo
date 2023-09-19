/*-----------------------------------------------------
    文件：AudioSvc.cs
	作者：Johnson
    日期：2023/4/27 18:35:6
	功能：声音播放服务
------------------------------------------------------*/

using UnityEngine;

public class AudioSvc : MonoBehaviour {
    public static AudioSvc Instance = null;
    public AudioSource bgAudio;
    public AudioSource uiAudio;

    public void InitSvc() {
        Instance = this;
        PECommon.Log("Init AudioSvc...");
    }

    public void StopBGMusic() {
        if (bgAudio != null) {
            bgAudio.Stop();
        }
    }

    public void PlayBGMusic(string name, bool isLoop = true) {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        //如果当前没播放音乐，或者播放的音乐不同，需要切换音乐
        if(bgAudio.clip == null || bgAudio.clip.name != audio.name) {
            bgAudio.clip = audio;
            bgAudio.loop = isLoop;
            bgAudio.Play();
        }
    }

    public void PlayUIAudio(string name) {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        uiAudio.clip = audio;
        uiAudio.Play();
    }

    public void PlayCharAudio(string name, AudioSource audioSrc) {
        AudioClip audio = ResSvc.Instance.LoadAudio("ResAudio/" + name, true);
        audioSrc.clip = audio;
        audioSrc.Play();
    }
}