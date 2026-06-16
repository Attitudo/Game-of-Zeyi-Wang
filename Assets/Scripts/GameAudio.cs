using UnityEngine;
using UnityEngine.SceneManagement;

public class GameAudio : MonoBehaviour
{
    private static GameAudio instance;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private AudioClip bgm;
    private AudioClip switchOn;
    private AudioClip mirrorRotate;
    private AudioClip mirrorSlide;
    private AudioClip receiverPower;
    private AudioClip empFire;
    private AudioClip guardAlert;
    private AudioClip caught;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (instance != null)
        {
            return;
        }

        GameObject go = new GameObject("GameAudio");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<GameAudio>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.volume = 0.22f;
        bgmSource.spatialBlend = 0f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.volume = 0.78f;
        sfxSource.spatialBlend = 0f;

        LoadClips();
        PlayBgm();
    }

    private void LoadClips()
    {
        bgm = Resources.Load<AudioClip>("Audio/bgm_relaxed");
        switchOn = Resources.Load<AudioClip>("Audio/switch_on");
        mirrorRotate = Resources.Load<AudioClip>("Audio/mirror_rotate");
        mirrorSlide = Resources.Load<AudioClip>("Audio/mirror_slide");
        receiverPower = Resources.Load<AudioClip>("Audio/receiver_power");
        empFire = Resources.Load<AudioClip>("Audio/emp_fire");
        guardAlert = Resources.Load<AudioClip>("Audio/guard_alert");
        caught = Resources.Load<AudioClip>("Audio/caught");
    }

    private void PlayBgm()
    {
        if (bgmSource == null || bgm == null)
        {
            return;
        }

        if (bgmSource.clip != bgm)
        {
            bgmSource.clip = bgm;
        }

        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    private void PlayClip(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null)
        {
            return;
        }

        sfxSource.PlayOneShot(clip, volume);
    }

    public static void PlaySwitch()
    {
        if (instance != null) instance.PlayClip(instance.switchOn, 0.85f);
    }

    public static void PlayMirrorRotate()
    {
        if (instance != null) instance.PlayClip(instance.mirrorRotate, 0.58f);
    }

    public static void PlayMirrorSlide()
    {
        if (instance != null) instance.PlayClip(instance.mirrorSlide, 0.42f);
    }

    public static void PlayReceiverPower()
    {
        if (instance != null) instance.PlayClip(instance.receiverPower, 0.86f);
    }

    public static void PlayEmpFire()
    {
        if (instance != null) instance.PlayClip(instance.empFire, 0.95f);
    }

    public static void PlayGuardAlert()
    {
        if (instance != null) instance.PlayClip(instance.guardAlert, 0.80f);
    }

    public static void PlayCaught()
    {
        if (instance != null) instance.PlayClip(instance.caught, 0.95f);
    }
}
