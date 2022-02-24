using System.Collections.Generic;
using UnityEngine;

public class Question : ScriptableObject
{
    public int id;
    public string category;
    public string questionText;
    public List<string> options;
    public int answer;
}
