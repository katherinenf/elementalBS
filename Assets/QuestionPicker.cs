using System;
﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IQuestion
{
    string GetQuestionText(Element target);
    Vector2Int GetAnswerRange();
    bool Evaluate(Element target, int userAnswer);
}

class GroupNumberQuestion : IQuestion
{
    public string GetQuestionText(Element target)
    {
        return "What is the group number of " + target.elementName + "?";
    }

    public Vector2Int GetAnswerRange()
    {
        return new Vector2Int(1, 18);
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

    public Vector2Int GetAnswerRange()
    {
        return new Vector2Int(1, 7);
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

    public Vector2Int GetAnswerRange()
    {
        return new Vector2Int(1, 8);
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

    public Vector2Int GetAnswerRange()
    {
        return new Vector2Int(1, 118);
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

    public Vector2Int GetAnswerRange()
    {
        return new Vector2Int(1, 7);
    }

    public bool Evaluate(Element target, int userAnswer)
    {
        return (userAnswer == target.energyShells);
    }
}

// provides the logic for selecting a question
public class QuestionPicker : MonoBehaviour
{
    IQuestion valenceQuestion;
    IQuestion[] pool;
    int gameSalt;

    void Awake()
    {
        valenceQuestion = new ValenceNumberQuestion();
        pool = new IQuestion[] {
            new GroupNumberQuestion(),
            new PeriodNumberQuestion(),
            new EnergyShellsNumberQuestion(),
            new AtomicNumberQuestion(),
            valenceQuestion
        };

        // Select a salt used for the entire game
        gameSalt = UnityEngine.Random.Range(0, Int32.MaxValue);
    }

    int BuildSeed(Element e)
    {
        int turnNum = GameplayManager.Instance.turnNum;
        int playerNum = (GameplayManager.Instance.phase == GameplayPhase.TurnPlayer1) ? 1 : 0;
        int elementNum = e.number;
        return gameSalt + (turnNum<<2) + (playerNum<<8) + (e.number<<16);
    }

    // returns a valid question for the provided element  while obeying optionally omitted questions
    public IQuestion Choose(Element e, List<IQuestion> omit = null)
    {
        if (omit == null)
        {
            omit = new List<IQuestion>();
        }

        // if element is in groups 3-12 8th graders will never need to determine the valance number
        if(e.group >= 3 && e.group <= 12)
        {
            omit.Add(valenceQuestion);
        }

        // Build the valid question pool by removing the omitted questions
        List<IQuestion> validPool = pool.Except(omit).ToList();

        // Seed Random so that each element will always choose the same questions on a turn
        UnityEngine.Random.InitState(BuildSeed(e));

        // Select and return a random valid question
        int index = UnityEngine.Random.Range(0, validPool.Count);
        return validPool[index];
    }
}
