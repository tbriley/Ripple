using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviourSingleton<Snake>
{
    public Vector2 InitPosition = new Vector2(10, 10);
    public float Speed = .5f;
    public float Acceleration = 1;

    public PrimitiveType PrimitiveType = PrimitiveType.Cube;

    public float Impulsion = -.05f;
    public float ImpulsionOverTime = .01f;

    private int _score = 0;
    public UnityEngine.UI.Text ScoreText;

    private Vector3 Direction = Vector3.right;
    private float _lastHeadCreation;

    private readonly List<GameObject>  _snake = new List<GameObject>();

    private bool _allowDirectionChange = true;

    public bool IncrementSize { get; set; }
    
    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject sphereGo = GameObject.CreatePrimitive(PrimitiveType);
            sphereGo.transform.position = new Vector3(InitPosition.x + i * GraphicMeshUpdater.Instance.Unit, 0, InitPosition.y);
            sphereGo.transform.localScale = new Vector3(1, 1, 1) * GraphicMeshUpdater.Instance.Unit;
            sphereGo.GetComponent<Renderer>().material.color = Color.red;
            sphereGo.transform.parent = transform;
            _snake.Add(sphereGo);
        }
    }

    void Update()
    {
        if (Time.time > _lastHeadCreation + Speed - Mathf.Max(0, Time.time * Acceleration))
            MoveSnake();

        CheckGameover();
        if (ItemManager.Instance.CollectItem(_snake.Last().transform.position))
        {
            _score++;
            ScoreText.text = string.Format("SCORE : {0}", _score);
        }
        UpdateSnakeTailHeights();
        UpdateInputs();
    }

    void CheckGameover()
    {
        var untransformedPosition = _snake.Last().transform.position;
        var p = untransformedPosition / GraphicMeshUpdater.Instance.Unit;

        if (Mathf.RoundToInt(p.x) < 0 || 
            Mathf.RoundToInt(p.z) < 0 || 
            Mathf.RoundToInt(p.x) > GraphicMeshUpdater.Instance.Size || 
            Mathf.RoundToInt(p.z) > GraphicMeshUpdater.Instance.Size)
        {
            GameOver();
        }

        for (int i = 0; i < _snake.Count - 1; i++)
        {
            var tr = _snake[i].transform;
            if (Math.Abs(tr.position.x - untransformedPosition.x) < float.Epsilon &&
                Math.Abs(tr.position.z - untransformedPosition.z) < float.Epsilon)
                GameOver();
        }
    }

    void GameOver()
    {

        Debug.LogError("GAMEOVER");
        Destroy(this);
        SceneManager.LoadScene(0);
    }

    void UpdateInputs()
    {
        if (!_allowDirectionChange)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow) && Direction != Vector3.right)
        {
            Direction = Vector3.left;
            _allowDirectionChange = false;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) && Direction != Vector3.left)
        {
            Direction = Vector3.right;
            _allowDirectionChange = false;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && Direction != Vector3.back)
        {
            Direction = Vector3.forward;
            _allowDirectionChange = false;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && Direction != Vector3.forward)
        {
            Direction = Vector3.back;
            _allowDirectionChange = false;
        }
    }

    void MoveSnake()
    {
        var headPosition = _snake.Last().transform.position;

        GameObject sphereGo = GameObject.CreatePrimitive(PrimitiveType);
        sphereGo.transform.position = headPosition + new Vector3(Direction.x * GraphicMeshUpdater.Instance.Unit, 0, Direction.z * GraphicMeshUpdater.Instance.Unit);
        sphereGo.transform.localScale = new Vector3(1, 1, 1) * GraphicMeshUpdater.Instance.Unit;
        sphereGo.GetComponent<Renderer>().material.color = Color.red;
        sphereGo.transform.parent = transform;
        _snake.Add(sphereGo);

        if (!IncrementSize)
        {
            Destroy(_snake.First());
            _snake.RemoveAt(0);
        }
        else
            IncrementSize = false;

        if (!_allowDirectionChange)
            _allowDirectionChange = true;

        _lastHeadCreation = Time.time;
    }

    void UpdateSnakeTailHeights()
    {
        foreach (var sphere in _snake)
        {
            var p = sphere.transform.position / GraphicMeshUpdater.Instance.Unit;

            var x = Mathf.RoundToInt(p.x);
            var z = Mathf.RoundToInt(p.z);

            p.y = GraphicMeshUpdater.Instance.GetHeight(x, z);

            sphere.transform.position = new Vector3(sphere.transform.position.x, p.y, sphere.transform.position.z);
            GraphicMeshUpdater.Instance.Flatten(z, x, Impulsion + (Time.time * ImpulsionOverTime));
        }
        GraphicMeshUpdater.Instance.UpdateVertexMap();
    }
}
