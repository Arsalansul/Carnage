using Unity.Entities;
using UnityEngine;

public class PlayAudioClipOnDamageAuthoring : MonoBehaviour
{
    public AudioClip AudioClip;
        
    private class Baker : Baker<PlayAudioClipOnDamageAuthoring>
    {
        public override void Bake(PlayAudioClipOnDamageAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new PlayAudioClipOnDamageData
            {
                AudioClip = authoring.AudioClip
            });
            SetComponentEnabled<PlayAudioClipOnDamageData>(entity, false);
        }
    }
}
public struct PlayAudioClipOnDamageData : IComponentData, IEnableableComponent
{
    public UnityObjectRef<AudioClip> AudioClip;
}