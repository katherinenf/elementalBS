using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IQuestion
{
    string GetQuestionText(Element target);
    bool Evaluate(Element target, int userAnswer);
}

class GroupNumberQuestion : IQuestion
{
    public string GetQuestionText(Element target)
    {
        return "What is the group number of " + target.elementName + "?";
    }

    public bool Evaluate(Element target, int userAnswer)
    {
        return (userAnswer == target.group);
    }
}

class PeriodNumberQuestion : IQuestion
{
    public string GetQuestionText(Element target)
    {
        return "What is the period number of " + target.elementName + "?";
    }

    public bool Evaluate(Element target, int userAnswer)
    {
        return (userAnswer == target.period);
    }
}

class ValenceNumberQuestion : IQuestion
{
    public string GetQuestionText(Element target)
    {
        return "How many valence electrons does " + target.elementName + " have?";
    }

    public bool Evaluate(Element target, int userAnswer)
    {
        return (userAnswer == target.valence);
    }
}

class AtomicNumberQuestion : IQuestion
{
    public string GetQuestionText(Element target)
    {
        return "What is the atomic number of " + target.elementName + "?";
    }

    public bool Evaluate(Element target, int userAnswer)
    {
        return (userAnswer == target.number);
    }
}

class EnergyShellsNumberQuestion : IQuestion
{
    public string GetQuestionText(Element target)
    {
        return "How many energy shells does " + target.elementName + " have?";
    }

    public bool Evaluate(Element target, int userAnswer)
    {
        return (userAnswer == target.energyShells);
    }
}

// provides the logic for selecting a question
public class QuestionPicker : MonoBehaviour
{
    IQuestion[] pool;

    void Awake()
    {
        pool = new IQuestion[] {
            new GroupNumberQuestion(),
            new PeriodNumberQuestion(),
            new EnergyShellsNumberQuestion(),
            new AtomicNumberQuestion(),
            new ValenceNumberQuestion()
        };
    }

    //returns a valid question for the provided element
    public IQuestion Choose(Element e)
    {
        int max = pool.Length;
        // if element is in groups 3-12 8th graders will never need to determine the valance number
        if(e.group >= 3 && e.group <= 12)
        {
            max = pool.Length - 1;
        }
        int index = Random.Range(0, max);
        return pool[index];
    }
}
