using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; //Singleton
    private List<Question> questions = new List<Question>();

    void Awake()
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
    }

    private void LoadQuestions()
    {
        Object[] objects = Resources.LoadAll("Questions/");
        foreach (Object question in objects)
        {
            questions.Add(question as Question);
        }
    }

    public List<Question> GetQuestions()
    {
        return questions;
    }

}
