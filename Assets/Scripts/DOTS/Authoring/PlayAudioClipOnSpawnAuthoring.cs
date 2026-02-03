using Unity.Entities;
using UnityEngine;

namespace DOTS.Authoring
{
    public class PlayAudioClipOnSpawnAuthoring : MonoBehaviour
    {
        public AudioClip AudioClip;

        private class Baker : Baker<PlayAudioClipOnSpawnAuthoring>
        {
            public override void Bake(PlayAudioClipOnSpawnAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new PlayAudioClipOnSpawnData
                {
                    AudioClip = authoring.AudioClip
                });
            }
        }
    }

    public struct PlayAudioClipOnSpawnData : IComponentData, IEnableableComponent
    {
        public UnityObjectRef<AudioClip> AudioClip;
    }
}