using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Ui.View
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private GameObject selectedGameObject;
        [SerializeField] private Text countText;

        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public void SetSelected(bool selected)
        {
            selectedGameObject.SetActive(selected);
        }

        public void SetCount(int count)
        {
            countText.text = count.ToString();
            countText.gameObject.SetActive(count > 1);
        }

        public class Pool : MemoryPool<Transform, InventorySlot>
        {
            protected override void OnCreated(InventorySlot item)
            {
                base.OnCreated(item);
                item.gameObject.SetActive(false);
            }

            protected override void Reinitialize(Transform parent, InventorySlot item)
            {
                base.Reinitialize(parent, item);
                item.transform.SetParent(parent);
            }

            protected override void OnDespawned(InventorySlot item)
            {
                base.OnDespawned(item);
                item.gameObject.SetActive(false);
            }

            protected override void OnDestroyed(InventorySlot item)
            {
                base.OnDestroyed(item);
            }

            protected override void OnSpawned(InventorySlot item)
            {
                base.OnSpawned(item);
                item.gameObject.SetActive(true);
            }
        }
    }
}