using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class QuestionGenerator
{

    private static string quizDataPath = "/Editor/QuizData/Data.csv";
    private static int maxOptionNumber = 4;

    [MenuItem("Tools/Generate Questions")]
    public static void GenerateQuestions()
    {
        //Delete previous questions
        string[] questionsFolder = { "Assets/Resources/Questions" };
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
                if (splitData.Length != maxOptionNumber + 4)
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
                    {
                        question.options.Add(splitData[i]);
                    }
                    else if (i >= 3 && i < maxOptionNumber + 2) //Check the number of blank options
                    {
                        Debug.LogError("Not Enough Options On Line Number " + lineNumber);
                        return;
                    }
                }
                question.answer = int.Parse(splitData[splitData.Length - 1]);

                AssetDatabase.CreateAsset(question, $"Assets/Resources/Questions/Question_{question.id.ToString("0000")}.asset");
            }

            lineNumber++;
        }

        AssetDatabase.SaveAssets();
    }

}
