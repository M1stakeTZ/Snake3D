using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public Sprite[] snakes;
    public Image[] snake;
    int[] sna = new int[3] { 0, 1, 2 };
    public void play()
    {
        SceneManager.LoadScene(1);
    }

    void Start()
    {
        if (!PlayerPrefs.HasKey("snakeColor")) PlayerPrefs.SetInt("snakeColor", 1);
        else changeSnake(PlayerPrefs.GetInt("snakeColor"));
    }

    public void changeSnake(int sn)
    {
        int randInt = 0;
        if (sn == 1) return;
        if (sn == 0)
        {
            randInt = sna[2];
            sna[2] = sna[1];
            sna[1] = sna[0];
            sna[0] = randInt;
        }
        else
        {
            randInt = sna[0];
            sna[0] = sna[1];
            sna[1] = sna[2];
            sna[2] = randInt;
        }

        for (int i = 0; i < 3; i++)
        {
            snake[i].sprite = snakes[sna[i]];
        }

        PlayerPrefs.SetInt("snakeColor", sna[1]);
    }
}
