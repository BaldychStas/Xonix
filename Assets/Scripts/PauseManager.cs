using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public Sprite pauseSprite;
    public Sprite playSprite;

    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private Image pauseImage;
    private  bool pause;

    public void OnPauseButtonClick()
    {
        pause = !pause;
        gameManager.SetPause(pause);
        if (pause)
            pauseImage.sprite = playSprite;
        else
            pauseImage.sprite = pauseSprite;
    }
}
