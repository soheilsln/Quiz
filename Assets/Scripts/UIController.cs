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

    [Header("Reslut Panel")]
    public GameObject resultPanel;
    public Button StartNewQuizButton;
    public Text rankingText;

    private string currentCategory;
    private List<Question> currentQuestions = new List<Question>();
    private int currentQuestionIndex = 0;
    private bool currentQuestionAnswered = false;

    public static event Action<bool> OnAnswered;

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
        List<string> categories = GameManager.instance.GetCategories();
        categoriesDropdown.ClearOptions();

        categoriesDropdown.AddOptions(categories);
    }

    private void OnStartQuizButtonClicked()
    {
        currentCategory = categoriesDropdown.options[categoriesDropdown.value].text;
        categoriesPanel.SetActive(false);

        currentQuestions = GameManager.instance.GetCategoryQuestions(currentCategory);
        UpdateQuizPanel();
        quizPanel.SetActive(true);
    }

    private void UpdateQuizPanel()
    {
        //Reset panel
        nextQuestionButton.gameObject.SetActive(false);
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
        StartCoroutine(StartTimer());

    }

    private IEnumerator StartTimer()
    {
        float maxTime = GameManager.instance.GetMaxTime();
        float time = maxTime;
        while (time >= 0 && !currentQuestionAnswered)
        {
            time -= Time.deltaTime;
            timer.fillAmount = time / maxTime;
            if (timer.fillAmount < 0.3f)
                timer.color = Color.red;

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private void OnOptionButtonClicked(int optionNumer)
    {
        //Activate next question and see results buttons based on current question
        currentQuestionAnswered = true;
        if (currentQuestionIndex + 1 < currentQuestions.Count)
        {
            nextQuestionButton.gameObject.SetActive(true);
            seeResultButton.gameObject.SetActive(false);
        }
        else
        {
            nextQuestionButton.gameObject.SetActive(false);
            seeResultButton.gameObject.SetActive(true);
        }

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

        float result = GameManager.instance.GetResult();
        if(result >= 0.75)
        {
            rankingText.text = "WOW !";
        }
        else if(result >= 0.5)
        {
            rankingText.text = "You are smart !";
        }
        else
        {
            rankingText.text = "You can do better!";
        }

        resultPanel.SetActive(true);
    }

}
