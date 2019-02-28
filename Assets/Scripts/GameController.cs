using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public MazeGenerator mazeGenerator;
    private GameObject marble;
    public Canvas GameOverUI;
    public Text gameOverText;
    bool gameOver;
    int minSizeX = 6;
    int minSizeY = 6;
    int maxSizeX = 10;
    int maxSizeY = 10;

	
    // Start is called before the first frame update
    void Start()
    {
        GameOverUI.enabled = false;
        gameOver = false;
        mazeGenerator.GenerateMaze(Random.Range(minSizeX, maxSizeX), Random.Range(minSizeY, maxSizeY));
        mazeGenerator.InstantiateMaze();
        marble = mazeGenerator.SpawnMarble();
    }

    // Update is called once per frame
    void Update()
    {
       if(marble != null && marble.transform.position.y < 1.5) {
            setGameOver();
            Destroy(marble);
        }

        if (Input.GetKey("q")) {
            Application.Quit();
        }
        if (Input.GetKey("f")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void setGameOver() {
        GameOverUI.enabled = true;
        gameOver = true;
        gameOverText.text = "Well done! \n Press F to Restart \n Press Q to Quit";

    }
}
