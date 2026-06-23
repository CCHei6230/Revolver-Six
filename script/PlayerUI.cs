using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class PlayerUI : MonoBehaviour
{

    [Header("Ammo")]
    public GameObject[] bullets;
    public GameObject reel;
    public Image[] bulletImages;
    public Image reelImages;
    public Image infinityImages;
    public Image HPImage;
    public Image[] BlackImage;
    public int bulletsSlotPresent = 1;
    [SerializeField] TextMeshProUGUI hpText;
    [SerializeField] Color originalHpColor;

    Tween rotationTween;

    void Start()
    {
        originalHpColor = HPImage.color;
        for (int i = 0; i < bullets.Length; i++)
        {
            bulletImages[i] = bullets[i].GetComponent<Image>();
        }
        reelImages = reel.GetComponent<Image>();

        BlackImage[0].gameObject.SetActive(true);
        BlackImage[0].fillOrigin = 0;
        BlackImage[0].DOFillAmount(0, 0.25f);
        BlackImage[0].DOColor(Color.clear, 0.25f);

        BlackImage[1].gameObject.SetActive(true);
        BlackImage[1].fillOrigin = 1;
        BlackImage[1].DOFillAmount(0, 0.25f);
        BlackImage[1].DOColor(Color.clear, 0.25f);
    }

    public void BlackScreen()
    {
        BlackImage[0].fillOrigin = 1;
        BlackImage[0].DOFillAmount(1, 0.25f);
        BlackImage[0].DOColor(Color.black, 0.25f);

        BlackImage[1].fillOrigin = 0;
        BlackImage[1].DOFillAmount(1, 0.25f);
        BlackImage[1].DOColor(Color.black, 0.25f);

    }

    public void HPUI(int _hp,bool _damage)
    {
        HPImage.color = originalHpColor;
        if (_damage)
        {
            HPImage.DOColor(Color.orangeRed, .1f)
                .OnStepComplete(()=> HPImage.DOColor(originalHpColor, .1f));
            HPImage.DOFillAmount(_hp/100.0f, .3f).SetEase(Ease.OutElastic);
        }
        else
        {
            HPImage.DOColor(new Color(0,0.8f,1,1), .1f)
                .OnStepComplete(()=> HPImage.DOColor(originalHpColor, .1f));
            HPImage.DOFillAmount(_hp/100.0f, 0.6f);

        }
        hpText.text = _hp.ToString() + "/100";
    }
    public void ReloadAmmo()
    {
        reel.transform.DOComplete();
        reel.transform.localRotation = Quaternion.Euler(0, 0,0);
        for (int i = 0; i < 6; i++)
        {
            bulletImages[i].color = new Color(0.7f, 0.6f, 0.2f, 0);
        }
        reelImages.DOColor(new Color(0.55f, 0.55f, 0.55f, 0.8f), 0.25f);
        reel.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.275f).SetEase(Ease.OutCirc);
        reel.transform.localRotation = Quaternion.Euler(0, 0,0);
        bulletsSlotPresent = 1;
    }
    public void RotateReel()
    {
        reel.transform.DOComplete();
        reel.transform.localRotation = Quaternion.Euler(0, 0,0);
        reel.transform.DOLocalRotate(new Vector3(0,0,540.0f), 0.5f)
            .OnComplete(()=>reel.transform.localRotation= Quaternion.Euler(0, 0, 0));
        reel.transform.DOScale(new Vector3(1,1,1), 0.5f).SetEase(Ease.OutExpo);

        foreach (var tmp_bullet in bulletImages)
        {
            tmp_bullet.DOColor(new Color(0.85f, 0.68f, 0.37f, 1f), 0.65f);
        }
        reelImages.DOColor(new Color(0.48f, 0.7f, 1, 1f), 0.65f);
    }
    public void Infinity( bool _isInfinity)
    {
        if (_isInfinity)
        {
            infinityImages.DOColor(Color.gold, 0.1f);
        }
        else
        {
            infinityImages.DOColor(Color.gray2*new Color(1,1,1,0.25f), 0.1f);
        }
    }

    public void BulletIncreaseAndDecrease(int _amout)
    {
        int bulletIndex = bulletsSlotPresent-1;
        if (_amout > 0)
        {

            bulletImages[bulletIndex].color = new Color(bulletImages[bulletIndex].color.r, bulletImages[bulletIndex].color.g, bulletImages[bulletIndex].color.b, 0.25f);
            bulletImages[bulletIndex].DOColor(new Color(0.7f, 0.6f, 0.2f, 0.55f), 0.2f);
            bullets[bulletIndex].transform.localScale = Vector3.one*3.0f;
            bullets[bulletIndex].transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InOutElastic)
                .OnStepComplete(() =>
                    reel.transform.DOLocalRotate(reel.transform.localRotation.eulerAngles + new Vector3(0, 0, 60.0f), .1f));
        }
        else if(_amout < 0)
        {
            bulletImages[bulletIndex].DOColor(  new Color(1f,0.5f,0f,0f), 0.15f);
            rotationTween = bullets[bulletIndex].transform.DOScale(Vector3.one * 2.0f, 0.1f).SetEase(Ease.OutBounce)
                .OnStepComplete(() =>
                    reel.transform.DOLocalRotate(reel.transform.localRotation.eulerAngles + new Vector3(0, 0, 60.0f), .1f));
        }
        bulletsSlotPresent++;

        if (bulletsSlotPresent > 6)
        {
            bulletsSlotPresent = 1;
        }
    }

}
