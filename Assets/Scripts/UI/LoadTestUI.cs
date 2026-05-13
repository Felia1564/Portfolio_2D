using UnityEngine;
using UnityEngine.UI;

public class LoadTestUI : MonoBehaviour
{
    [SerializeField] private Image image_LoadSampleImage;

    private void OnEnable()
    {
        Sprite loadedSprite = Resources.Load<Sprite>("PopUp8bit");
        if (loadedSprite != null)
        {
            image_LoadSampleImage.sprite = loadedSprite;
        }
    }
}
