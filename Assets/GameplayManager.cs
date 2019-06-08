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

public class GameplayManager : MonoBehaviour
{
    public GameObject player1Fleet;

    public GameObject player2Fleet;

    public Text turnText;
    
    public Text victoryText;

    public GameObject victoryScreen;


    // Currently active game phase
    private GameplayPhase phase;

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
        turnText.text = "Player 1's Turn to Place";
        SetVisableFleet(1);
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
        turnText.text = "Player 2's Turn to Place";

    }

    void SetVisableFleet(int playerNum)
    {
        player1Fleet.SetActive(playerNum == 1);
        player2Fleet.SetActive(playerNum == 2);

    }
    
    void StartTurn(int playerNum)
    {
        Debug.Log("starting player turn " + playerNum);
        int opPlayer = ToOppositePlayer(playerNum);
        SetVisableFleet(opPlayer);
        PeriodicTable table = GetFleetForPlayer(opPlayer).GetComponentInChildren<PeriodicTable>();
        table.SetBombingEnabled(true);
        table.OnStartTurn();
        phase = (playerNum == 1) ? GameplayPhase.TurnPlayer1 : GameplayPhase.TurnPlayer2;
        turnText.text = "Player " + playerNum + "'s Turn";

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
    }

    public void ReturnToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}
