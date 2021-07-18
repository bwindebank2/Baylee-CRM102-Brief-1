using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject playerOneCanvas;
    public TMP_Text playerOneScoreText;
    public Color playerOneColour;

    public GameObject playerTwoCanvas;
    public TMP_Text playerTwoScoreText;
    public Color playerTwoColour;

    /// <summary>
    /// hide canbas when we first start the game
    /// </summary>
    public void DisplayScores(bool displayScores)
    {
        if (playerOneCanvas == null || playerTwoCanvas == null)
        {
            Debug.Log("No canvas has been assigned");
        }

        playerOneCanvas.SetActive(displayScores);
        playerTwoCanvas.SetActive(displayScores);
    }

    public void UpdateScores(int playerOneScore, int playerTwoScore)
    {
        if(playerOneScoreText == null || playerTwoScoreText == null)
        {
            Debug.Log("player score text has not been assigned");
        }

        playerOneScoreText.color = playerOneColour; // change the color of our text
        playerOneScoreText.text = playerOneScore.ToString(); // set text to player score

        playerTwoScoreText.color = playerTwoColour; // change the color of our text
        playerTwoScoreText.text = playerTwoScore.ToString(); // set text to player score
    }
}
