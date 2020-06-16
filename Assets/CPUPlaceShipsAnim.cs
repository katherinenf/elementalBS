using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CPUPlaceShipsAnim : MonoBehaviour
{
    public RectTransform[] ships;
    public Sprite[] handSprites;
    public RectTransform hand;

    // Start is called before the first frame update
    void Start()
    {
        Sequence cutscene = DOTween.Sequence();
        cutscene
            .Append(HandGrab(ships[0]))
            .AppendInterval(.5f)
            .Append(HandGrab(ships[1]))
            .AppendInterval(.5f)
            .Append(HandGrab(ships[2]));
    }

    Sequence HandGrab(RectTransform ship)
    {
        Image img = hand.GetComponentInChildren<Image>();
        return DOTween.Sequence()
            .AppendCallback(() =>
            {
                hand.anchoredPosition = new Vector2(ship.anchoredPosition.x - 150f, 700.0f);
                img.sprite = handSprites[0];
                img.ChangeAlpha(0.0f);
            })
            .Append(hand.DOAnchorPos(new Vector3(0, -935.0f), 1.5f)
                .SetEase(Ease.OutCubic).SetRelative(true))
            .Join(img.DOFade(1.0f, 1.0f))
            .AppendCallback(() =>
            {
                img.sprite = handSprites[1];
                ship.SetParent(hand.transform);
            })
            .Append(hand.DOAnchorPos(new Vector3(0, 935.0f), 1f)
                .SetEase(Ease.InQuad).SetRelative(true))
            .Join(img.DOFade(0.0f, 1f))
            .Join(ship.GetComponent<Image>().DOFade(0.0f, 1f))
            .AppendCallback(() => ship.gameObject.SetActive(false));
    }
}
