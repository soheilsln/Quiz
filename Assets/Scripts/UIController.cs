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

    private string currentCategory;
    private List<Question> currentQuestions = new List<Question>();
    private int currentQuestionIndex = 0;

    private void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        startQuizButton.onClick.AddListener(OnStartQuizButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        startPanel.SetActive(false);

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
        questionText.text = currentQuestions[currentQuestionIndex].questionText;
        int numberOfOptions = currentQuestions[currentQuestionIndex].options.Count;
        for (int i = 0; i < numberOfOptions; i++)
        {
            options[i].GetComponentInChildren<Text>().text = currentQuestions[currentQuestionIndex].options[i];
            options[i].gameObject.SetActive(true);
        }


        currentQuestionIndex++;
    }

}
