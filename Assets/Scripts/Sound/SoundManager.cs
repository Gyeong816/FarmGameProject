using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource ambienceSource;

    [Header("SFX Clips (PlayOneShot)")]
    [SerializeField] private List<AudioClip>      sfxClips;
    [Header("BGM Clips (루프 재생)")]
    [SerializeField] private List<AudioClip>      bgmClips;
    [Header("Ambience Clips (e.g. rain, wind…)")]
    [SerializeField] private List<AudioClip>      ambienceClips;

    private Dictionary<string, AudioClip> sfxDict;
    private Dictionary<string, AudioClip> bgmDict;
    private Dictionary<string, AudioClip> ambienceDict;

    private void Awake()
    {
        // 싱글턴 처리
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);



        // 기본 설정
        sfxSource.playOnAwake      = false;
        bgmSource.playOnAwake      = false;
        bgmSource.loop             = true;
        ambienceSource.playOnAwake = false;
        ambienceSource.loop        = true;

        InitSfxDict();
        InitBgmDict();
        InitAmbienceDict();

        // BGM 자동 재생 (리스트의 첫 번째)
        if (bgmClips != null && bgmClips.Count > 0)
            PlayBgm(bgmClips[0].name);
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PlaySfx("ClickSound");
        }
    }
    
    private void InitSfxDict()
    {
        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var clip in sfxClips)
            if (clip != null && !sfxDict.ContainsKey(clip.name))
                sfxDict.Add(clip.name, clip);
    }

    private void InitBgmDict()
    {
        bgmDict = new Dictionary<string, AudioClip>();
        foreach (var clip in bgmClips)
            if (clip != null && !bgmDict.ContainsKey(clip.name))
                bgmDict.Add(clip.name, clip);
    }

    private void InitAmbienceDict()
    {
        ambienceDict = new Dictionary<string, AudioClip>();
        foreach (var clip in ambienceClips)
            if (clip != null && !ambienceDict.ContainsKey(clip.name))
                ambienceDict.Add(clip.name, clip);
    }

    /// <summary>
    /// 효과음 재생 (PlayOneShot)
    /// </summary>
    public void PlaySfx(string clipName, float volume = 1f)
    {
        if (sfxDict.TryGetValue(clipName, out var clip))
            sfxSource.PlayOneShot(clip, volume);
        else
            Debug.LogWarning($"[SoundManager] SFX '{clipName}' not found.");
    }

    /// <summary>
    /// 지정된 이름의 BGM 루프 재생
    /// </summary>
    public void PlayBgm(string clipName)
    {
        if (!bgmDict.TryGetValue(clipName, out var clip))
        {
            Debug.LogWarning($"[SoundManager] BGM '{clipName}' not found.");
            return;
        }
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    /// <summary>
    /// 현재 재생 중인 BGM 정지
    /// </summary>
    public void StopBgm() => bgmSource.Stop();

    /// <summary>
    /// 지정된 이름의 환경음(loop) 재생
    /// </summary>
    public void PlayAmbience(string clipName, float volume)
    {
        if (!ambienceDict.TryGetValue(clipName, out var clip))
        {
            Debug.LogWarning($"[SoundManager] Ambience '{clipName}' not found.");
            return;
        }
        if (ambienceSource.clip == clip && ambienceSource.isPlaying) return;

        ambienceSource.clip = clip;
        ambienceSource.volume = volume;
        ambienceSource.Play();
    }

    /// <summary>
    /// 환경음 정지
    /// </summary>
    public void StopAmbience() => ambienceSource.Stop();
}
