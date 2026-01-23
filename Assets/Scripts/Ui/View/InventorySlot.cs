using UnityEngine;
using UnityEngine.UI;

namespace Ui.View
{
    public class InventorySlot : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private GameObject selectedGameObject;

        public void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public void SetSelected(bool selected)
        {
            selectedGameObject.SetActive(selected);
        }
    }
}