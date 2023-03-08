using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private GameObject cardBack;
    [SerializeField] private SpriteRenderer spriteRender;
    private bool isVisible = false;

    public void SetFaceVisible(bool faceVisible)
    {
        isVisible = faceVisible;
        // back should be active if face is not visable
        cardBack.SetActive(!faceVisible);
    }

    public bool IsFaceVisible()
    {
        return isVisible;
    }

    private void OnMouseDown()
    {
        Messenger<Card>.Broadcast(GameEvent.CARD_CLICKED, this); // broadcasts the click event for the scenelistener
    }

    public void SetSprite(Sprite image)
    {
        spriteRender.sprite = image;
    }

    public Sprite GetSprite()
    {
        return spriteRender.sprite;
    }
}
