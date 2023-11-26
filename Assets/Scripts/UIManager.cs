using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject startUI;
    [SerializeField] private GameObject endUI;
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject failUI;
    [SerializeField] private GameObject[] progressImages;
    [SerializeField] private int progressImageIndex;
    [SerializeField] private TextMeshProUGUI currentLevelText;
    [SerializeField] private TextMeshProUGUI nextLevelText;
    [SerializeField] private TextMeshProUGUI gameUITotalGemText;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI scorePopUp;
    [SerializeField] private GameObject gemImageGameUI;
    [SerializeField] private GameObject gemImageEndUI;
    [SerializeField] private Image gemSpawn;
    [SerializeField] private TextMeshProUGUI endUIGemRewardText;
    [SerializeField] private TextMeshProUGUI endUITotalGemText;
    [SerializeField] private int rewardGem;
    [SerializeField] private int totalGem;
    [SerializeField] private Button continueButton;
    [SerializeField] private Image tapMeterFillImage;
    [SerializeField] private GameObject rampUI;
    [SerializeField] private TextMeshProUGUI tapPowerPercentage;

    private void Start()
    {
        ShowNextLevel();
        totalGem = gameManager.gemCount;
        gameUITotalGemText.text = totalGem.ToString();
    }

    public void LevelStart()
    {
        startUI.SetActive(false);
        gameUI.SetActive(true);
        gameUITotalGemText.text = totalGem.ToString();
    }

    public void FillProgressImage()
    {
        progressImages[progressImageIndex].SetActive(true);
        progressImageIndex++;
    }

    public void ShowNextLevel()
    {
        int currentLevel = gameManager.level;
        currentLevelText.text = currentLevel.ToString();
        nextLevelText.text = (currentLevel + 1).ToString();
        ResetProgressImages();
    }

    private void ShowScorePopUp(int score)
    {
        scorePopUp.text = "+" + score;
        scorePopUp.rectTransform.localPosition = new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), 0);
        scorePopUp.gameObject.SetActive(true);
        scorePopUp.transform.DOScale(1, 0.5f).OnComplete(() =>
        {
            scorePopUp.DOFade(0, 1).OnComplete(() =>
            {
                scorePopUp.gameObject.SetActive(false);
                scorePopUp.alpha = 1;
                scorePopUp.transform.localScale = Vector3.zero;
            });
        });
    }

    public IEnumerator ShowGemPopUps(int score)
    {
        ShowScorePopUp(score);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 10; i++)
        {
            Sequence gemSequence = DOTween.Sequence();
            Image gemPopUp = ObjectPool.SharedInstance.GetPooledObject(4).GetComponent<Image>();
            if (gemPopUp != null)
            {
                gemPopUp.rectTransform.localPosition = new Vector3(Random.Range(-400, 400), Random.Range(-600, 600), 0);
                gemPopUp.gameObject.SetActive(true);
                gemSequence.Append(gemPopUp.transform.DOScale(new Vector3(1, 0.8f, 1), 0.2f));
                gemSequence.Append(gemPopUp.transform.DOMove(gemImageGameUI.transform.position, 1f));
                gemSequence.OnComplete(() =>
                {
                    gemPopUp.gameObject.SetActive(false);
                    DOTween.Kill(gemPopUp.transform);
                });
            }
            yield return new WaitForSeconds(0.04f);
        }
        yield return new WaitForSeconds(1.2f);
        gemImageGameUI.transform.DOPunchScale(new Vector3(0, 0.5f, 0), 0.5f, 2).OnComplete(() =>
        {
            totalGem += score;
            gameUITotalGemText.text = totalGem.ToString();
        });
    }

    public void ShowEndUI(int gemReward, int gemTotal)
    {
        rewardGem = gemReward;
        totalGem = gemTotal;
        endUIGemRewardText.text = rewardGem.ToString();
        endUITotalGemText.text = totalGem.ToString();
        gameUI.SetActive(false);
        endUI.SetActive(true); 
    }

    public void ContinueButton()
    {
        StartCoroutine(ContinueButtonRoutine());
    }

    private IEnumerator ContinueButtonRoutine()
    {
        continueButton.gameObject.SetActive(false);
        int minimumStep = (int)rewardGem / 10;
        for (int i = 0; i < 10; i++)
        {
            Sequence gemSequence = DOTween.Sequence();
            Image gemPopUp = ObjectPool.SharedInstance.GetPooledObject(4).GetComponent<Image>();
            if (gemPopUp != null)
            {
                int randomStep = Random.Range(minimumStep, minimumStep + 5);
                if (rewardGem < randomStep)
                {
                    randomStep = rewardGem;
                    rewardGem = 0;
                }
                else
                {
                    rewardGem -= randomStep;
                }
                endUIGemRewardText.text = rewardGem.ToString();
                gemPopUp.transform.localScale = new Vector3(1, 0.8f, 1);
                gemPopUp.rectTransform.position = gemSpawn.rectTransform.position;
                gemPopUp.gameObject.SetActive(true);
                gemSequence.Append(gemPopUp.transform.DOMove(gemImageEndUI.transform.position, 2f));
                gemSequence.Insert(0, gemPopUp.transform.DOScale(Vector3.zero, gemSequence.Duration()));
                gemSequence.Insert(0, gemPopUp.transform.DORotate(new Vector3(0, 0, 360), gemSequence.Duration(), RotateMode
                    .FastBeyond360));
                gemSequence.OnComplete(() =>
                {
                    totalGem += randomStep;
                    endUITotalGemText.text = totalGem.ToString();
                    gemImageEndUI.transform.DOPunchScale(Vector3.one / 5, 0.1f, 2, 0.5f);
                    gemPopUp.gameObject.SetActive(false);
                    DOTween.Kill(gemPopUp.transform);
                });
            }
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(2.5f);
        gameUITotalGemText.text = totalGem.ToString();
        endUI.SetActive(false);
        continueButton.gameObject.SetActive(true);
        startUI.SetActive(true);
        gameManager.EnablePlayer();
    }

    public void ToggleRampUI()
    {
        rampUI.SetActive(!rampUI.activeSelf);
    }

    public void UpdateTapMeter(float power, float maxPower)
    {
        float level = power / maxPower;
        tapMeterFillImage.fillAmount = level;
        tapPowerPercentage.text = "%" + (int)power;
    }

    private void ResetProgressImages()
    {
        for (int i = 0; i < progressImages.Length; i++)
        {
            progressImages[i].SetActive(false);
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
}
