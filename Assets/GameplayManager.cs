using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameplayPhase
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

    public TextMeshPro phaseText;

    public Text victoryText;

    public GameObject placementHelp;

    public GameObject bombHelp;

    public GameObject victoryScreen;

    public GameObject[] hiddenAfterPlacement;

    public GameObject[] shownAfterPlacement;

    public SpriteRenderer topBanner;

    public GameObject turnHider;

    public Text turnHiderTitle;

    // Currently active game phase
    public GameplayPhase phase;

    public int turnNum;

    private static GameplayManager instance;
    private static readonly Color Player1Color = new Color(1/255.0f, 105/255.0f, 223/255.0f, .9f);
    private static readonly Color Player2Color = new Color(191/255.0f, 1/255.0f, 1/255.0f, .7f);

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
        turnText.text = "";
        phaseText.text = "<color=#7AE0FF>Player 1</color>'s Turn to Place";
        topBanner.color = Player1Color;
        SetVisibleTable(1);
        SetHelpText(HelpText.Placement);
        ShowTurnHider("<color=#7AE0FF>Player 1</color>'s Turn to Place");
    }

    // updates scene to allow player2 to place ships
    void GoToPlacementPlayer2()
    {
        // show turnHider
        ShowTurnHider("<color=#ff7a7a>Player 2</color>'s Turn to Place");

        // deactivate player1 placement and activate player2 placement
        SetVisibleTable(2);

        // update game phase
        phase = GameplayPhase.PlacementPlayer2;
        topBanner.color = Player2Color;
        turnText.text = "";
        phaseText.text = "<color=#ff7a7a>Player 2</color>'s Turn to Place";

    }

    void SetVisibleTable(int playerNum)
    {
        player1Fleet.SetActive(playerNum == 1);
        player2Fleet.SetActive(playerNum == 2);
    }

    // shows the provided help text and hides all of the others
    void SetHelpText(HelpText help)
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

        Debug.Log("Starting player " + playerNum + "'s turn " + turnNum);
        phase = (playerNum == 1) ? GameplayPhase.TurnPlayer1 : GameplayPhase.TurnPlayer2;

        // Swap visible tables
        int opPlayer = ToOppositePlayer(playerNum);
        SetVisibleTable(opPlayer);

        // Prep the table
        PeriodicTable table = GetTableForPlayer(opPlayer);
        table.OnStartTurn();

        // Update the UI texts
        string colorHex = (playerNum == 1) ? "#7AE0FF" : "#ff7a7a";
        turnText.text = "Turn " + turnNum;
        topBanner.color = (playerNum == 1) ? Player1Color : Player2Color;
        phaseText.text = "<color=" + colorHex + ">Player " + playerNum + "</color>'s Turn";
        SetHelpText(HelpText.Bomb);

        // Enable the black screen
        ShowTurnHider("<color=" + colorHex + ">Player " + playerNum + "</color>'s Turn");
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
            // Hide some specific objects after placement ends
            foreach (GameObject obj in hiddenAfterPlacement)
            {
                obj.SetActive(false);
            }

            // Show some specific objects after placement ends
            foreach (GameObject obj in shownAfterPlacement)
            {
                obj.SetActive(true);
            }

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

    PeriodicTable GetTableForPlayer(int playerNum)
    {
        if (playerNum == 1)
        {
            return player1Fleet.GetComponentInChildren<PeriodicTable>();
        }
        else
        {
            return player2Fleet.GetComponentInChildren<PeriodicTable>();
        }
    }

    public PeriodicTable GetCurrentTable()
    {
        switch (phase)
        {
            case GameplayPhase.PlacementPlayer1:
            case GameplayPhase.TurnPlayer2: return GetTableForPlayer(1);
            case GameplayPhase.PlacementPlayer2:
            case GameplayPhase.TurnPlayer1: return GetTableForPlayer(2);
            default: return null;
        }
    }

    public void Victory()
    {
        int playerNum = (phase == GameplayPhase.TurnPlayer1) ? 1 : 2;
        phase = GameplayPhase.Victory;
        victoryText.text = "Player " + playerNum + " wins!";
        victoryScreen.SetActive(true);
        SetHelpText(HelpText.None);
    }

    public void ReturnToMenu()
    {
        Input.ResetInputAxes();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    // called when player clicks turn hider before turn begins
    public void OnTurnHiderClicked()
    {
        Input.ResetInputAxes();
        turnHider.SetActive(false);

        if (phase >= GameplayPhase.TurnPlayer1)
        {
            PeriodicTable table = GetCurrentTable();
            table.SetBombingEnabled(true);
        }
    }

    void ShowTurnHider(string title)
    {
        turnHider.SetActive(true);
        turnHiderTitle.text = title;
    }
}
