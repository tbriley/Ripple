using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour
{
    public GraphicMeshUpdater Water;
    public Vector2 InitPosition = new Vector2(10, 10);
    public float Speed = .5f;
    public float IncrementSizeSpeed = 3;

    public PrimitiveType PrimitiveType = PrimitiveType.Cube;

    public float Impulsion = -.05f;

    private Vector3 Direction = Vector3.right;
    private float _lastHeadCreation;
    private float _lastIncrementTime;

    private readonly List<GameObject>  _snake = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject sphereGo = GameObject.CreatePrimitive(PrimitiveType);
            sphereGo.transform.position = new Vector3(InitPosition.x + i * Water.Unit, 0, InitPosition.y);
            sphereGo.transform.localScale = new Vector3(1, 1, 1) * Water.Unit;
            sphereGo.GetComponent<Renderer>().material.color = Color.red;
            sphereGo.transform.parent = transform;
            _snake.Add(sphereGo);
        }
    }

    void Update()
    {
        if (Time.time > _lastHeadCreation + Speed)
        {
            MoveSnake(Time.time > _lastIncrementTime + IncrementSizeSpeed);
            _lastHeadCreation = Time.time;
        }

        UpdateSnakeTailHeights();

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Direction = Vector3.left;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Direction = Vector3.right;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            Direction = Vector3.forward;
        if (Input.GetKeyDown(KeyCode.DownArrow))
            Direction = Vector3.back;
    }

    void MoveSnake(bool incrementSize = false)
    {
        var headPosition = _snake.Last().transform.position;

        GameObject sphereGo = GameObject.CreatePrimitive(PrimitiveType);
        sphereGo.transform.position = headPosition + new Vector3(Direction.x * Water.Unit, 0, Direction.z * Water.Unit);
        sphereGo.transform.localScale = new Vector3(1, 1, 1) * Water.Unit;
        sphereGo.GetComponent<Renderer>().material.color = Color.red;
        sphereGo.transform.parent = transform;
        _snake.Add(sphereGo);

        if (!incrementSize)
        {
            Destroy(_snake.First());
            _snake.RemoveAt(0);
        }
        else
        {
            _lastIncrementTime = Time.time;
        }
    }

    void UpdateSnakeTailHeights()
    {
        foreach (var sphere in _snake)
        {
            var p = sphere.transform.position / Water.Unit;

            var x = Mathf.RoundToInt(p.x);
            var z = Mathf.RoundToInt(p.z);

            p.y = Water.GetHeight(x, z);

            sphere.transform.position = new Vector3(sphere.transform.position.x, p.y, sphere.transform.position.z);
            Water.Impulse(z, x, Impulsion);
        }
        Water.UpdateVertexMap();
    }
}
