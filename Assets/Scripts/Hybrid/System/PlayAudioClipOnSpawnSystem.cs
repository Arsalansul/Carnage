using Unity.Burst;
using Unity.Entities;

public partial struct PlayAudioClipOnSpawnSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (audioClipData, playAudioClip) in SystemAPI.Query<PlayAudioClipOnSpawnData, EnabledRefRW<PlayAudioClipOnSpawnData>>())
        {
            var audioSource = PoolManager.Instance.GetAudioSource();
            audioSource.clip = audioClipData.AudioClip;
            audioSource.Play();
            PoolManager.Instance.ReturnAudioSourceToPool(audioSource, audioClipData.AudioClip.Value.length);
            playAudioClip.ValueRW = false;
        }
    }
}