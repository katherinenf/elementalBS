using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum GameplayPhase
{
    PlacementPlayer1,
    PlacementPlayer2,
    TurnPlayer1,
    TurnPlayer2,
    Victory
}

enum HelpText
{
    Placement,
    Bomb,
    None
}

public class GameplayManager : MonoBehaviour
{
    public GameObject player1Fleet;

    public GameObject player2Fleet;

    public Text turnText;

    public Text phaseText;

    public Text victoryText;

    public GameObject placementHelp;

    public GameObject bombHelp;

    public GameObject victoryScreen;

    // Currently active game phase
    private GameplayPhase phase;

    private int turnNum;

    private static GameplayManager instance;

    public static GameplayManager Instance
    {
        get { return instance; }
    }


    private void Awake()
    {
        GameplayManager.instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        phase = GameplayPhase.PlacementPlayer1;
        phaseText.text = "<color=#7AE0FF>Player 1</color>'s Turn to Place";
        SetVisableFleet(1);
        ShowHelpText(HelpText.Placement);
    }

    // Update is called once per frame
    void Update()
    {

    }

    // updates scene to allow player2 to place ships
    void GoToPlacementPlayer2()
    {
        // deactivate player1 placement and activate player2 placement
        SetVisableFleet(2);

        // update game phase
        phase = GameplayPhase.PlacementPlayer2;
        phaseText.text = "<color=#7AE0FF>Player 2</color>'s Turn to Place";

    }

    void SetVisableFleet(int playerNum)
    {
        player1Fleet.SetActive(playerNum == 1);
        player2Fleet.SetActive(playerNum == 2);
    }

    // shows the provided help text and hides all of the others
    void ShowHelpText(HelpText help)
    {
        switch(help)
        {
        case HelpText.Placement:
            placementHelp.SetActive(true);
            bombHelp.SetActive(false);
            break;
        case HelpText.Bomb:
            placementHelp.SetActive(false);
            bombHelp.SetActive(true);
            break;
        case HelpText.None:
            placementHelp.SetActive(false);
            bombHelp.SetActive(false);
            break;
        }
    }

    void StartTurn(int playerNum)
    {
        // Increment turn count only at the beginning of player 1's turn
        if (playerNum == 1)
        {
            turnNum++;
        }

        Debug.Log("starting player turn " + playerNum);
        int opPlayer = ToOppositePlayer(playerNum);
        SetVisableFleet(opPlayer);
        PeriodicTable table = GetFleetForPlayer(opPlayer).GetComponentInChildren<PeriodicTable>();
        table.SetBombingEnabled(true);
        table.OnStartTurn();
        phase = (playerNum == 1) ? GameplayPhase.TurnPlayer1 : GameplayPhase.TurnPlayer2;
        turnText.text = "Turn " + turnNum;
        phaseText.text = "<color=#7AE0FF>Player " + playerNum + "</color>'s Turn";
        ShowHelpText(HelpText.Bomb);
    }

    // invoked by done button during ship placement
    public void ShipPlacementComplete()
    {
        if (phase == GameplayPhase.PlacementPlayer1)
        {
            GoToPlacementPlayer2();
        }
        else if (phase == GameplayPhase.PlacementPlayer2)
        {
            StartTurn(1);
        }
        else
        {
            Debug.LogError("ShipPlacementComplete called in invalid phase");
        }
    }

    public void EndTurn()
    {
        if (phase == GameplayPhase.TurnPlayer1)
        {
            StartTurn(2);
        }
        else
        {
            StartTurn(1);
        }
    }

    // 1 --> 2, 2 --> 1
    int ToOppositePlayer(int playerNum)
    {
        return playerNum % 2 + 1;
    }

    GameObject GetFleetForPlayer(int playerNum)
    {
        if (playerNum == 1)
        {
            return player1Fleet;
        }
        else
        {
            return player2Fleet;
        }
    }

    public void Victory()
    {
        int playerNum = (phase == GameplayPhase.TurnPlayer1) ? 1 : 2;
        phase = GameplayPhase.Victory;
        victoryText.text = "Player " + playerNum + " wins!";
        victoryScreen.SetActive(true);
        ShowHelpText(HelpText.None);
    }

    public void ReturnToMenu()
    {
        Input.ResetInputAxes();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}
