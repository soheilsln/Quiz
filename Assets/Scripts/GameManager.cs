using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; //Singleton
    private List<Question> questions = new List<Question>();
    [SerializeField]
    private float maxTime = 10f;
    public float MaxTime { get { return maxTime; } }
    public CoinManager coinManager;

    private int currentNumberOfQuestions;
    private int result;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        LoadQuestions();
        coinManager = this.GetComponent<CoinManager>();
    }

    private void Start()
    {
        UIController.OnAnswered += this.OnAnswered;
    }

    private void OnDestroy()
    {
        UIController.OnAnswered -= this.OnAnswered;
    }

    private void LoadQuestions()
    {
        //Adding all questions to the questions list
        Object[] objects = Resources.LoadAll("Questions/");
        foreach (Object question in objects)
        {
            questions.Add(question as Question);
        }
    }

    public List<string> GetCategories()
    {
        //return all unique categories
        List<string> categories = new List<string>();

        foreach (Question question in questions)
        {
            if (!categories.Contains(question.category))
                categories.Add(question.category);
        }

        return categories;
    }

    public List<Question> GetCategoryQuestions(string category)
    {
        //return all questions in the category
        result = 0;
        List<Question> categoryQuestions = new List<Question>();

        foreach (Question question in questions)
        {
            if (question.category == category)
            {
                categoryQuestions.Add(question);
            }
        }

        currentNumberOfQuestions = categoryQuestions.Count;
        return categoryQuestions;
    }

    private void OnAnswered(bool value)
    {
        //Increment result by 1 when answered correctly
        if (value)
            result++;
    }

    public float GetFinalResult()
    {
        //Compute the result percentage
        float r = (float)result / (float)currentNumberOfQuestions;
        return r;
    }

}
