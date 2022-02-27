using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Start Panel")]
    public GameObject startPanel;
    public Button startButton;

    [Header("Categories Panel")]
    public GameObject categoriesPanel;
    public Dropdown categoriesDropdown;
    public Button startQuizButton;

    [Header("Quiz Panel")]
    public GameObject quizPanel;
    public Text questionText;
    public Image timer;
    public Button[] options;
    public Button nextQuestionButton;
    public Button seeResultButton;
    public Button fiftyFiftyButton;
    public Text coinsText;

    [Header("Reslut Panel")]
    public GameObject resultPanel;
    public Button StartNewQuizButton;
    public Text rankingText;
    public Text resultCoinsText;

    private string currentCategory;
    private List<Question> currentQuestions = new List<Question>();
    private int currentQuestionIndex = 0;
    private bool currentQuestionAnswered = false;
    private GameManager gameManager;
    private CoinManager coinManager;

    public static event Action<bool> OnAnswered;
    public static event Action FiftyFiftyClicked;
    public static event Action<int> OnQuizFinished;

    private void Awake()
    {
        gameManager = GameManager.instance;
        coinManager = gameManager.coinManager;
    }

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        startQuizButton.onClick.AddListener(OnStartQuizButtonClicked);
        for (int i = 0; i < options.Length; i++)
        {
            int optionNumber = i;
            options[i].onClick.AddListener(delegate { OnOptionButtonClicked(optionNumber); });
        }
        nextQuestionButton.onClick.AddListener(OnNextQuestionButtonClicked);
        seeResultButton.onClick.AddListener(OnSeeResultButtonClicked);
        StartNewQuizButton.onClick.AddListener(OnStartButtonClicked);
        fiftyFiftyButton.onClick.AddListener(OnFiftyFiftyButtonClicked);
        fiftyFiftyButton.GetComponentInChildren<Text>().text = "50/50 (" + coinManager.FiftyFiftyCoins + " coins)";
    }

    private void OnStartButtonClicked()
    {
        startPanel.SetActive(false);
        resultPanel.SetActive(false);

        currentQuestionIndex = 0;
        CreateCategories();
        categoriesPanel.SetActive(true);
    }

    private void CreateCategories()
    {
        //Add categories to dropdown menu
        List<string> categories = gameManager.GetCategories();
        categoriesDropdown.ClearOptions();

        categoriesDropdown.AddOptions(categories);
    }

    private void OnStartQuizButtonClicked()
    {
        currentCategory = categoriesDropdown.options[categoriesDropdown.value].text;
        categoriesPanel.SetActive(false);

        currentQuestions = gameManager.GetCategoryQuestions(currentCategory);
        UpdateQuizPanel();
        quizPanel.SetActive(true);
    }

    private void UpdateQuizPanel()
    {
        //Reset panel
        nextQuestionButton.gameObject.SetActive(false);
        fiftyFiftyButton.gameObject.SetActive(true);

        if (coinManager.Coins >= coinManager.FiftyFiftyCoins)
            fiftyFiftyButton.interactable = true;
        else
            fiftyFiftyButton.interactable = false;

        SetCoinsText();
        currentQuestionAnswered = false;
        foreach (Button option in options)
        {
            option.image.color = Color.white;
            option.gameObject.SetActive(false);
        }

        //Update panel based on the current question
        questionText.text = currentQuestions[currentQuestionIndex].questionText;
        int numberOfOptions = currentQuestions[currentQuestionIndex].options.Count;
        for (int i = 0; i < numberOfOptions; i++)
        {
            options[i].GetComponentInChildren<Text>().text = currentQuestions[currentQuestionIndex].options[i];
            options[i].gameObject.SetActive(true);
        }
        ToggleOptions(true);
        StartCoroutine(StartTimer());

    }

    private IEnumerator StartTimer()
    {
        timer.color = Color.green;
        float maxTime = gameManager.MaxTime;
        float time = maxTime;
        while (time >= 0 && !currentQuestionAnswered)
        {
            time -= Time.deltaTime;
            timer.fillAmount = time / maxTime;
            if (timer.fillAmount < 0.33f)
                timer.color = Color.red;
            else if (timer.fillAmount < 0.66f)
                timer.color = new Color(1f, 0.84f, 0f);//Orange

            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (time < 0)
        {
            options[currentQuestions[currentQuestionIndex].answer - 1].image.color = Color.green;
            if (OnAnswered != null)
                OnAnswered(false);
            ActivateNextQuestion();
            ToggleOptions(false);
        }
    }

    private void OnOptionButtonClicked(int optionNumer)
    {
        //Activate next question and see results buttons based on current question
        currentQuestionAnswered = true;
        ActivateNextQuestion();

        //Change the color of buttons based on the answer
        if (currentQuestions[currentQuestionIndex].answer == optionNumer + 1)
        {
            options[optionNumer].image.color = Color.green;
            if (OnAnswered != null)
                OnAnswered(true);
        }
        else
        {
            options[optionNumer].image.color = Color.red;
            options[currentQuestions[currentQuestionIndex].answer - 1].image.color = Color.green;
            if (OnAnswered != null)
                OnAnswered(false);
        }

        //Disable options after question answered
        ToggleOptions(false);
    }

    private void ActivateNextQuestion()
    {
        if (currentQuestionIndex + 1 < currentQuestions.Count)
        {
            nextQuestionButton.gameObject.SetActive(true);
            seeResultButton.gameObject.SetActive(false);
            fiftyFiftyButton.gameObject.SetActive(false);
        }
        else
        {
            nextQuestionButton.gameObject.SetActive(false);
            seeResultButton.gameObject.SetActive(true);
            fiftyFiftyButton.gameObject.SetActive(false);
        }
    }

    private void ToggleOptions(bool value)
    {
        //Enable or Disable options
        foreach (Button option in options)
        {
            option.interactable = value;
        }
    }

    private void OnNextQuestionButtonClicked()
    {
        currentQuestionIndex++;
        UpdateQuizPanel();
    }

    private void OnSeeResultButtonClicked()
    {
        seeResultButton.gameObject.SetActive(false);
        quizPanel.SetActive(false);

        float result = gameManager.GetFinalResult();
        if (result >= 0.75)
        {
            rankingText.text = "WOW !\nMore than 75% correct answers!" +
                " (" + coinManager.FirstRankCoins + " coins)";
            if (OnQuizFinished != null)
                OnQuizFinished(1);
        }
        else if (result >= 0.5)
        {
            rankingText.text = "You are smart !\nMore than 50% correct answers!" +
                " (" + coinManager.SecondRankCoins + " coins)";
            if (OnQuizFinished != null)
                OnQuizFinished(2);
        }
        else
        {
            rankingText.text = "You can do better!\nLess than 50% correct answers!" +
                " (" + coinManager.ThirdRankCoins + " coins)";
            if (OnQuizFinished != null)
                OnQuizFinished(3);
        }

        SetCoinsText();
        resultPanel.SetActive(true);
    }

    private void OnFiftyFiftyButtonClicked()
    {
        fiftyFiftyButton.gameObject.SetActive(false);
        int numberOfOptions = currentQuestions[currentQuestionIndex].options.Count;
        int answer = currentQuestions[currentQuestionIndex].answer - 1;
        int otherOption = answer;

        while (otherOption == answer)
        {
            otherOption = UnityEngine.Random.Range(0, numberOfOptions);
        }

        for (int i = 0; i < numberOfOptions; i++)
        {
            if (i != answer && i != otherOption)
            {
                options[i].gameObject.SetActive(false);
            }
        }

        if (FiftyFiftyClicked != null)
            FiftyFiftyClicked();
        SetCoinsText();
    }

    private void SetCoinsText()
    {
        coinsText.text = "Coins = " + coinManager.Coins;
        resultCoinsText.text = "Coins = " + coinManager.Coins;
    }

}
