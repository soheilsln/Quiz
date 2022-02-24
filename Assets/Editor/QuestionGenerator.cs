using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class QuestionGenerator
{

    private static string quizDataPath = "/Editor/QuizData/Data.csv";
    private static int maxOptionNumber = 4;

    [MenuItem("Utilities/Generate Questions")]
    public static void GenerateQuestions()
    {
        //Delete previous questions
        string[] questionsFolder = { "Assets/Questions" };
        foreach (string asset in AssetDatabase.FindAssets("", questionsFolder))
        {
            string path = AssetDatabase.GUIDToAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }

        string[] alllines = File.ReadAllLines(Application.dataPath + quizDataPath);

        int lineNumber = 1;
        foreach (string line in alllines)
        {
            if (lineNumber > 1) //ignore first line
            {
                //Split each line with comma
                string[] splitData = line.Split(',');

                //Check the number of values
                if (splitData.Length != 8)
                {
                    Debug.LogError("Not Enough Values On Line Number " + lineNumber);
                    return;
                }

                //Create the question scriptable object from data file
                Question question = ScriptableObject.CreateInstance<Question>();
                question.id = int.Parse(splitData[0]);
                question.category = splitData[1];
                question.questionText = splitData[2];
                question.options = new List<string>();
                for (int i = 3; i < maxOptionNumber + 3; i++)
                {
                    if (splitData[i] != "")
                        question.options.Add(splitData[i]);
                }
                question.answer = int.Parse(splitData[splitData.Length - 1]);

                AssetDatabase.CreateAsset(question, $"Assets/Questions/question{question.id}.asset");
            }

            lineNumber++;
        }

        AssetDatabase.SaveAssets();
    }

}