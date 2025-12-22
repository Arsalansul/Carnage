using UnityEngine;
using Zenject;

namespace Core
{
    public class AudioController : MonoBehaviour
    {
        [Inject] private HybridHandler hybridHandler;
        [Inject] private PoolManager poolManager;
        
        private void Update()
        {
            if (hybridHandler.IsPlayClipOnDamage(out var clipOnDamage))
            {
                var audioSource = poolManager.GetAudioSource();
                audioSource.clip = clipOnDamage;
                audioSource.Play();
                poolManager.ReturnAudioSourceToPool(audioSource, audioSource.clip.length);
            }
            if (hybridHandler.IsPlayClipOnSpawn(out var clipOnSpawn))
            {
                var audioSource = poolManager.GetAudioSource();
                audioSource.clip = clipOnSpawn;
                audioSource.Play();
                poolManager.ReturnAudioSourceToPool(audioSource, audioSource.clip.length);
            }
        }
    }
}