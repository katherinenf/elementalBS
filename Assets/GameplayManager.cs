using System.Collections;
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

    public TurnHider turnHider;

    // Currently active game phase
    public GameplayPhase phase;

    public int turnNum;

    public OpponentAI opponentAI;

    private bool phaseComplete;

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
        if (GameConfig.Instance.useAI)
        {
            opponentAI = new OpponentAI();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RunGameLoop());
    }

    IEnumerator RunGameLoop()
    {
        // Let both players place their ships
        yield return RunPlacementPhase(1);
        yield return RunPlacementPhase(2);

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

        // Loop bombing turns until victory is achieved
        while (true)
        {
            // Update turn cpunt text each go-around
            turnText.text = "Turn " + ++turnNum;

            // Let player 1 bomb
            yield return RunBombingPhase(1);

            // Exit while loop if player 1 wins
            if (CheckVictoryCondition(1))
            {
                break;
            }

            // Let player 2 bomb
            yield return RunBombingPhase(2);

            // Exit while loop if player 2 wins
            if (CheckVictoryCondition(2))
            {
                break;
            }
        }

        // Show the victory screen
        ShowVictory();
    }

    // updates scene to allow player2 to place ships
    IEnumerator RunPlacementPhase(int playerNum)
    {
        phase = (playerNum == 1) ? GameplayPhase.PlacementPlayer1 : GameplayPhase.PlacementPlayer2;
        SetHelpText(HelpText.Placement);

        if (playerNum == 2 && opponentAI != null)
        {
            opponentAI.PlaceShips(GetTableForPlayer(2));
            yield return turnHider.ShowForSeconds(GetPlayerName(playerNum) + " is placing their ships", 5);
            EndPhase();
        }
        else
        {
            SetVisibleTable(playerNum);
            SetHeader(playerNum, phase);
            yield return turnHider.ShowUntilClick(GetPlayerName(playerNum) + "'s Turn to Place");

            // Wait for player end phase
            phaseComplete = false;
            while (!phaseComplete)
            {
                yield return null;
            }
        }
    }

    IEnumerator RunBombingPhase(int playerNum)
    {
        phase = (playerNum == 1) ? GameplayPhase.TurnPlayer1 : GameplayPhase.TurnPlayer2;

        // Swap visible tables
        int opPlayer = ToOppositePlayer(playerNum);
        SetVisibleTable(opPlayer);

        // Prep the table
        PeriodicTable table = GetTableForPlayer(opPlayer);
        table.OnStartTurn(playerNum == 2 && opponentAI != null);

        // Update the UI texts
        SetHeader(playerNum, phase);
        if (playerNum == 2 && opponentAI != null)
        {
            SetHelpText(HelpText.None);
            yield return turnHider.ShowForSeconds(GetPlayerName(playerNum) + "'s Turn", 2);
            yield return opponentAI.BombTile(table);
            EndPhase();
        } 
        else
        {
            SetHelpText(HelpText.Bomb);
            yield return turnHider.ShowUntilClick(GetPlayerName(playerNum) + "'s Turn");
            table.SetBombingEnabled(true);

            // Wait for player end phase
            phaseComplete = false;
            while (!phaseComplete)
            {
                yield return null;
            }
        }
    }

    bool CheckVictoryCondition(int playerNum)
    {
        int opPlayer = ToOppositePlayer(playerNum);
        PeriodicTable table = GetTableForPlayer(opPlayer);
        return table.AllShipsDestroyed();
    }

    public void ShowVictory()
    {
        int playerNum = (phase == GameplayPhase.TurnPlayer1) ? 1 : 2;
        phase = GameplayPhase.Victory;
        victoryText.text = GetPlayerName(playerNum) + " wins!";
        victoryScreen.SetActive(true);
        SetHelpText(HelpText.None);
    }

    public void EndPhase()
    {
        phaseComplete = true;
    }

    void SetVisibleTable(int playerNum)
    {
        player1Fleet.SetActive(playerNum == 1);
        player2Fleet.SetActive(playerNum == 2);
    }

    // shows the provided help text and hides all of the others
    void SetHelpText(HelpText help)
    {
        switch (help)
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

    void SetHeader(int playerNum, GameplayPhase phase)
    {
        switch (phase)
        {
            case GameplayPhase.PlacementPlayer1: // intentional fall-through
            case GameplayPhase.PlacementPlayer2:
            {
                phaseText.text = GetPlayerName(playerNum) + "'s Turn to Place";
                topBanner.color = (playerNum == 1) ? Player1Color : Player2Color;
                break;

            }
            case GameplayPhase.TurnPlayer1:
            case GameplayPhase.TurnPlayer2:
            {
                phaseText.text = GetPlayerName(playerNum) + "'s Turn";
                topBanner.color = (playerNum == 1) ? Player1Color : Player2Color;
                break;

            }
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

    public void ReturnToMenu()
    {
        Input.ResetInputAxes();
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }

    string GetPlayerName(int playerNum)
    {
        string colorHex = (playerNum == 1) ? "#7AE0FF" : "#ff7a7a";
        if (playerNum == 2 && opponentAI !=null)
        {
            return "The <color=" + colorHex + ">Computer</color>";
        }
        return "<color=" + colorHex + ">Player " + playerNum + "</color>";
    }
}
