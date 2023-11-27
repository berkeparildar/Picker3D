using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject failUI;
    [SerializeField] private GameObject rampUI;
    
    [SerializeField] private TextMeshProUGUI gameUICurrentLevelText;
    [SerializeField] private TextMeshProUGUI gameUINextLevelText;
    [SerializeField] private TextMeshProUGUI gameUITotalGemText;
    [SerializeField] private GameObject gameUIGemImage;
    [SerializeField] private GameObject[] gameUIProgressImages;
    [SerializeField] private int progressImageIndex;
        
    [SerializeField] private TextMeshProUGUI scorePopUp;
    [SerializeField] private TextMeshProUGUI endUIGemRewardText;
    [SerializeField] private GameObject endUIGemImage;
    [SerializeField] private TextMeshProUGUI endUITotalGemText;
    [SerializeField] private RectTransform endUIGemImageTarget;
    [SerializeField] private Button continueButton;
    
    [SerializeField] private Image rampUIFillImage;
    [SerializeField] private TextMeshProUGUI rampUIPowerPercentage;

    [SerializeField] private int gemPopUpCount = 10;
    [SerializeField] private Vector3 defaultGemImageScale;

    private void Start()
    {
        UpdateLevelIndicators();
        gameUITotalGemText.text = gameManager.gemCount.ToString();
    }

    public void SetProgressImage()
    {
        gameUIProgressImages[progressImageIndex].SetActive(true);
        progressImageIndex++;
    }

    public void UpdateLevelIndicators()
    {
        int currentLevel = gameManager.level;
        gameUICurrentLevelText.text = currentLevel.ToString();
        gameUINextLevelText.text = (currentLevel + 1).ToString();
        ResetProgressImages();
    }

    public IEnumerator ShowScorePopUp(int score)
    {
        yield return new WaitForSeconds(1);
        scorePopUp.text = "+" + score;
        scorePopUp.rectTransform.localPosition = GetRandomPosition(-200, 200, -200, 200);
        scorePopUp.gameObject.SetActive(true);
        Sequence scoreSequence = DOTween.Sequence();
        scoreSequence.Append(scorePopUp.transform.DOScale(1, 0.5f));
        scoreSequence.Append(scorePopUp.DOFade(0, 1));
        scoreSequence.OnComplete(() =>
        {
            scorePopUp.gameObject.SetActive(false);
            scorePopUp.alpha = 1;
            scorePopUp.transform.localScale = Vector3.zero;
        });
    }

    public IEnumerator ShowGemPopUp()
    {
        yield return new WaitForSeconds(2);
        float waitDuration = 0;
        for (int i = 0; i < gemPopUpCount; i++)
        {
            GameObject gemPopUp = ObjectPool.SharedInstance.GetPooledImage();
            if (gemPopUp != null)
            {
                waitDuration = GemImagePopAnimation(gemPopUp);
            }
        }
        yield return new WaitForSeconds(waitDuration);
        gameUIGemImage.transform.DOPunchScale(new Vector3(0, 0.5f, 0), 0.5f, 2);
        gameUITotalGemText.text = gameManager.gemCount.ToString();
    }
    
    private float GemImagePopAnimation(GameObject gemImage)
    {
        Sequence gemSequence = DOTween.Sequence();
        gemImage.transform.localPosition = GetRandomPosition(-400, 400, -600, 600);
        gemImage.gameObject.SetActive(true);
        gemSequence.Append(gemImage.transform.DOScale(defaultGemImageScale, 0.2f));
        gemSequence.Append(gemImage.transform.DOMove(gameUIGemImage.transform.position, 1f));
        gemSequence.OnComplete(() =>
        {
            gemImage.gameObject.SetActive(false);
            DOTween.Kill(gemImage.transform);
        });
        return gemSequence.Duration();
    }

    private float GemRotatingFadeAnimation(GameObject gemImage, int currentTotalGem)
    {
        Sequence gemSequence = DOTween.Sequence();
        gemImage.transform.localPosition = endUIGemImage.transform.localPosition;
        gemImage.gameObject.SetActive(true);
        gemSequence.Append(gemImage.transform.DOMove(endUIGemImageTarget.position, 2f));
        gemSequence.Insert(0, gemImage.transform.DOScale(Vector3.one / 5, gemSequence.Duration()));
        gemSequence.Insert(0, gemImage.transform.DORotate(new Vector3(0, 0, 360), gemSequence.Duration(), RotateMode
            .FastBeyond360));
        gemSequence.OnComplete(() =>
        {
            endUITotalGemText.text = currentTotalGem.ToString();
            endUIGemImage.transform.DOPunchScale(Vector3.one / 5, 0.1f, 2, 0.5f);
            DOTween.Kill(gemImage.transform);
            gemImage.gameObject.SetActive(false);
        });
        return gemSequence.Duration();
    }
    
    public void ContinueButton()
    {
        StartCoroutine(ContinueToStart());
    }

    private IEnumerator ContinueToStart()
    {
        yield return StartCoroutine(CollectRewardedGems());
        gameManager.ReadyPlayer();
    }

    private IEnumerator CollectRewardedGems()
    {
        continueButton.gameObject.SetActive(false);
        float waitDuration = 0;
        int rewardedGems = int.Parse(endUIGemRewardText.text);
        int currentTotalGems = int.Parse(endUITotalGemText.text);
        int minimumStep = rewardedGems / gemPopUpCount;
        for (int i = 0; i < gemPopUpCount; i++)
        {
            GameObject gemPopUp = ObjectPool.SharedInstance.GetPooledImage();
            if (gemPopUp != null)
            {
                int randomStep = Random.Range(minimumStep, minimumStep + 5);
                if (rewardedGems < randomStep)
                {
                    randomStep = rewardedGems;
                }
                rewardedGems -= randomStep;
                endUIGemRewardText.text = rewardedGems.ToString();
                currentTotalGems += randomStep;
                waitDuration = GemRotatingFadeAnimation(gemPopUp, currentTotalGems);
            }
            yield return new WaitForSeconds(waitDuration / gemPopUpCount);
        }
        yield return new WaitForSeconds(waitDuration);
        gameUITotalGemText.text = currentTotalGems.ToString();
        ShowStartUI();
    }

    public void UpdateTapMeter(float power, float maxPower)
    {
        float level = power / maxPower;
        rampUIFillImage.fillAmount = level;
        rampUIPowerPercentage.text = "%" + (int)power;
    }

    private void ResetProgressImages()
    {
        for (int i = 0; i < gameUIProgressImages.Length; i++)
        {
            gameUIProgressImages[i].SetActive(false);
        }
        progressImageIndex = 0;
    }

    public void RestartLevel()
    {
        ResetProgressImages();
        failUI.SetActive(false);
        startUI.SetActive(true);
    }

    public void ShowFailUI()
    {
        gameUI.SetActive(false);
        failUI.SetActive(true);
    }
    
    public void ShowGameUI()
    {
        startUI.SetActive(false);
        gameUI.SetActive(true);
        gameUITotalGemText.text = gameManager.gemCount.ToString();
    }

    public void ShowStartUI()
    {
        endUI.SetActive(false);
        continueButton.gameObject.SetActive(true);
        startUI.SetActive(true);
    }
    
    public void ShowEndUI(int gemReward, int gemTotal)
    {
        endUIGemRewardText.text = gemReward.ToString();
        endUITotalGemText.text = gemTotal.ToString();
        gameUI.SetActive(false);
        endUI.SetActive(true); 
    }
    
    public void ToggleRampUI()
    {
        rampUI.SetActive(!rampUI.activeSelf);
    }

    private Vector3 GetRandomPosition(int xMin, int xMax, int yMin, int yMax)
    {
        int randomXPosition = Random.Range(xMin, xMax);
        int randomYPosition = Random.Range(yMin, yMax);
        return new Vector3(randomXPosition, randomYPosition, 0);
    }
}
