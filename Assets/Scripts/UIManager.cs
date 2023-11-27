using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    [SerializeField] private GameObject startUI; // screen that shows up at the beginning of every level, goes with first tap
    [SerializeField] private GameObject endUI; // screen that shows after successfully completing a level
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject failUI; // screen that shows after failing a level
    [SerializeField] private GameObject rampUI; // tap meter and tap text
    
    [SerializeField] private TextMeshProUGUI gameUICurrentLevelText;
    [SerializeField] private TextMeshProUGUI gameUINextLevelText;
    [SerializeField] private TextMeshProUGUI gameUITotalGemText;
    [SerializeField] private GameObject gameUIGemImage;
    [SerializeField] private GameObject[] gameUIProgressImages; // these are the images between the levels at top
    [SerializeField] private int progressImageIndex;
        
    [SerializeField] private TextMeshProUGUI scorePopUp; // shows after landing on the landing zone
    [SerializeField] private TextMeshProUGUI endUIGemRewardText;
    [SerializeField] private GameObject endUIGemImage;
    [SerializeField] private TextMeshProUGUI endUITotalGemText;
    [SerializeField] private RectTransform endUIGemImageTarget;
    [SerializeField] private Button continueButton;
    
    [SerializeField] private Image rampUIFillImage;
    [SerializeField] private TextMeshProUGUI rampUIPowerPercentage;

    [SerializeField] private int gemPopUpCount = 10;
    // for some reason gem images I use look funny with their normal scale so I needed to add this
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

    // Updates the level texts and progress images. Called after a new level
    public void UpdateLevelIndicators()
    {
        int currentLevel = gameManager.level;
        gameUICurrentLevelText.text = currentLevel.ToString();
        gameUINextLevelText.text = (currentLevel + 1).ToString();
        ResetProgressImages();
    }

    // Shows the landed tile's score on screen at a random position, then fades away
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

    // Called slightly after the score pop up. Gems appear at random positions and move towards the upper left corner
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
    
    // Tween sequence that scales and moves an image. Called for gem images that appear after landing
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

    // Tween sequence that rotates, shrinks and moves gem images.
    // This is called at the end UI, after player hits continue button
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

    // Method that returns a random vector3 in the given bounds. Called when placing
    // pop up gem images randomly at the end of the level
    private Vector3 GetRandomPosition(int xMin, int xMax, int yMin, int yMax)
    {
        int randomXPosition = Random.Range(xMin, xMax);
        int randomYPosition = Random.Range(yMin, yMax);
        return new Vector3(randomXPosition, randomYPosition, 0);
    }
}
