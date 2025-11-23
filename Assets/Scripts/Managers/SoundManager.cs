using Cysharp.Threading.Tasks;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<SoundManager>();
                _instance ??= new GameObject("SoundManager").AddComponent<SoundManager>();
            }

            return _instance;
        }
    }

    private static SoundManager _instance;
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = FindObjectOfType<AudioSource>();
        _audioSource ??= gameObject.AddComponent<AudioSource>();
    }

    public async UniTask Play(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
        await UniTask.WaitUntil(() => !_audioSource.isPlaying);
    }
}