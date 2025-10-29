using Unity.Burst;
using Unity.Entities;
using UnityEngine;

[UpdateAfter(typeof(PlayAudioClipOnDamageEnableSystem))]
public partial struct PlayAudioClipOnDamageSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (audioClipData, playAudioClip) in SystemAPI.Query<PlayAudioClipOnDamageData, EnabledRefRW<PlayAudioClipOnDamageData>>())
        {
            var audioSource = PoolManager.Instance.GetAudioSource();
            audioSource.clip = audioClipData.AudioClip;
            audioSource.Play();
            PoolManager.Instance.ReturnAudioSourceToPool(audioSource, 1);
            playAudioClip.ValueRW = false;
        }
    }
}