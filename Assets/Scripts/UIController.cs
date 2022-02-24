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

    private List<Question> questions = new List<Question>();

    private void Start()
    {
        questions = GameManager.instance.GetQuestions();
        startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        startPanel.SetActive(false);
        categoriesPanel.SetActive(true);
    }

}
