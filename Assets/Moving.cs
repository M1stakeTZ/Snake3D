using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Moving : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 10f;
    public Transform rotator;
    float[] cameraRot = new float[3] { 0f, 0f, 0f };
    bool canCameraRot = false;
    int rot = 0;
    bool canDo = true;
    public float times = 1f;

    public GameObject snakePrefab;
    public GameObject parentPrefab;
    GameObject[] snake = new GameObject[200];
    int[] snakePos = new int[200];
    Vector3[] rotPos = new Vector3[100];
    bool[] canMove = new bool[200];
    int count = 0;

    public GameObject score;
    public GameObject applePrefab;
    int colorType = 1;
    bool isGameEnd = false;

    public GameObject finalPoints;
    public GameObject uLose;
    public GameObject againButon;
    public GameObject maxPoints;
    public GameObject[] arrows;
    GameObject apple;

    void Update()
    {
        if (isGameEnd) return;

        if (transform.position.x > 2.8f || transform.position.x < -2.8f || transform.position.y > 5.8f || transform.position.y < 0.2f || transform.position.z > 2.8f || transform.position.z < -2.8f)
        {
            gameEnd();
        }
        if (canDo)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow)) rot = 1;
            if (Input.GetKeyDown(KeyCode.LeftArrow)) rot = 2;
            if (Input.GetKeyDown(KeyCode.UpArrow)) rot = 3;
            if (Input.GetKeyDown(KeyCode.DownArrow)) rot = 4;
        }

        if (canCameraRot)
        {
            if (Mathf.Abs(cameraRot[0]) <= 5 && Mathf.Abs(cameraRot[1]) <= 5 && Mathf.Abs(cameraRot[2]) <= 5)
            {
                canCameraRot = false;
                rotator.rotation = new Quaternion(0, 0, 0, 0);
                cameraRot = new float[3] { 0, 0, 0 };
            }

            rotator.Rotate(znak(cameraRot[0]) * rotationSpeed * Time.deltaTime, znak(cameraRot[1]) * rotationSpeed * Time.deltaTime, znak(cameraRot[2]) * rotationSpeed * Time.deltaTime);
            cameraRot[0] -= znak(cameraRot[0]) * rotationSpeed * Time.deltaTime;
            cameraRot[1] -= znak(cameraRot[1]) * rotationSpeed * Time.deltaTime;
            cameraRot[2] -= znak(cameraRot[2]) * rotationSpeed * Time.deltaTime;
        }

        if (rot != 0)
        {
            if (rot == 1)
            {
                transform.Rotate(0, 90, 0);
                rotator.Rotate(0, -90, 0);
                cameraRot[1] += 90;
                if (apple != null) apple.transform.Rotate(0, 90, 0);
            }
            else if (rot == 2)
            {
                transform.Rotate(0, -90, 0);
                rotator.Rotate(0, 90, 0);
                cameraRot[1] -= 90;
                if (apple != null) apple.transform.Rotate(0, -90, 0);
            }
            else if (rot == 3)
            {
                transform.Rotate(0, 0, 90);
                rotator.Rotate(0, 0, -90);
                cameraRot[2] += 90;
                if (apple != null) apple.transform.Rotate(0, 0, 90);
            }
            else
            {
                transform.Rotate(0, 0, -90);
                rotator.Rotate(0, 0, 90);
                cameraRot[2] -= 90;
                if (apple != null) apple.transform.Rotate(0, 0, -90);
            }

            rot = 0;
            newPoint();
            canCameraRot = true;
            canDo = false;
            Invoke("doIt", .6f);
        }

        for (int i = 0; i < count + 1; i++)
        {
            if (canMove[i]) snake[i].transform.Translate(Vector3.right * Time.deltaTime * speed);

            if (snakePos[i] != -1) if (Vector3.Distance(snake[i].transform.position, rotPos[snakePos[i]]) <= 0.05f)
                {
                    snake[i].transform.position = rotPos[snakePos[i]];
                    snakePos[i]--;
                    Vector3 target;
                    if (snakePos[i] >= 0) target = rotPos[snakePos[i]];
                    else target = transform.position;

                    snake[i].transform.LookAt(target);
                    snake[i].transform.Rotate(0, -90, 0);

                    if (snakePos[i - 1] == snakePos[i]) if (Vector3.Distance(snake[i].transform.position, snake[i - 1].transform.position) < 1f)
                        {
                            snake[i].transform.position = snake[i].transform.position - (snake[i].transform.position - snake[i - 1].transform.position).normalized *
                                (Vector3.Distance(snake[i].transform.position, snake[i - 1].transform.position) - 1f);
                        }
                }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        for (int i = 0; i < count + 1; i++)
        {
            if (snake[i] == collision.gameObject)
            {
                if (i > 2) gameEnd();
                return;
            }
        }

        Destroy(apple);
        newSnake();
        genApple();
    }

    int znak(float i)
    {
        if (i > 0) i = 1;
        else if (i < 0) i = -1;
        return (int)i;
    }

    void doIt() { canDo = true; }

    void genApple()
    {
        for (int i = 0; i < 10000; i++)
        {
            float x = Random.Range(-2.5f, 2.5f);
            float y = Random.Range(0.5f, 5.5f);
            float z = Random.Range(-2.5f, 2.5f);
            bool can = true;

            for (int j = 0; j < count + 1; j++)
            {
                if (Vector3.Distance(snake[j].transform.position, new Vector3(x, y, z)) <= 1.1f) { can = false; break; }
            }

            if (!can) { continue; }

            apple = Instantiate(applePrefab, new Vector3(x, y, z), new Quaternion(0, 0, 0, 0));
            apple.transform.rotation = transform.rotation;
            break;
        }
    }

    void newSnake()
    {
        count++;
        if (count >= 4) score.GetComponent<Text>().text = "Ñ÷¸ò: " + (count - 3).ToString();
        snake[count] = Instantiate(snakePrefab, snake[count - 1].transform);
        snake[count].transform.SetParent(parentPrefab.transform);
        if (colorType == 2) snake[count].GetComponent<Renderer>().material.color = new Color((float)Random.Range(0, 255) / 255f, (float)Random.Range(0, 255) / 255f, (float)Random.Range(0, 255) / 255f);
        else if (colorType == 1)
        {
            if ((float)count % 2f == 0f) snake[count].GetComponent<Renderer>().material.color = new Color(0f, 1f, 0f);
        }
        else snake[count].GetComponent<Renderer>().material.color = new Color(0f, 1f, 0f);
        snakePos[count] = snakePos[count - 1];
        canMove[count] = false;
        Invoke("mustMove", times);
        
    }

    void mustMove()
    {
        for (int i = 0; i < 200; i++)
        {
            if (canMove[i] == false) { canMove[i] = true; break; }
        }
    }

    private void Start()
    {
        for (int i = 0; i < rotPos.Length; i++)
        {
            rotPos[i] = new Vector3(0, -1, 0);
        }

        colorType = PlayerPrefs.GetInt("snakeColor");

        GetComponent<Renderer>().material.color = new Color(0, 1, 0);

        snake[0] = gameObject;
        snakePos[0] = -1;
        canMove[0] = true;

        for (int i = 0; i < 3; i++) { Invoke("newSnake", (times) * i); }

        Invoke("genApple", (times) * 3);
    }

    void newPoint()
    {
        rotPos[rotPos.Length - 3] = new Vector3(0, -1, 0);
        int max = 0;
        for (int i = 0; i < rotPos.Length; i++)
        {
            if (rotPos[i].y != -1) max++;
        }

        for (int i = max; i >= 0; i--)
        {
            rotPos[i + 1] = rotPos[i]; 
        }

        rotPos[0] = transform.position;

        for (int i = 1; i < count + 1; i++) { snakePos[i]++; }
    }

    void gameEnd()
    {
        for (int i = 0; i < 4; i++) arrows[i].SetActive(false);
        isGameEnd = true;
        score.SetActive(false);
        uLose.SetActive(true);
        finalPoints.GetComponent<Text>().text = "Ñ÷¸ò: " + (count - 3).ToString();
        finalPoints.SetActive(true);
        againButon.SetActive(true);
        if (PlayerPrefs.HasKey("max"))
        {
            if (PlayerPrefs.GetInt("max") < count - 3) PlayerPrefs.SetInt("max", count - 3);
        }
        else PlayerPrefs.SetInt("max", count - 3);
        maxPoints.SetActive(true);
        maxPoints.GetComponent<Text>().text = "Ðåêîðä: " + PlayerPrefs.GetInt("max").ToString();
    }

    public void again()
    {
        SceneManager.LoadScene(0);
    }

    public void setRot(int rotat)
    {
        if (canDo) rot = rotat;
    }
}
