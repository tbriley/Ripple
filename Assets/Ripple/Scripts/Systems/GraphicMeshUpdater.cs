using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class GraphicMeshUpdater : MonoBehaviourSingleton<GraphicMeshUpdater>
{
    public int          Size = 10;
    public float        Spread = .1f;
    public float        Unit = 1;

    [Range(0, 0.1f)]
    public float        Damping = 0.04f;

    public float        SpringConstant = .02f;

    private float[]     _velocityMap;
    private float[]     _accelerationMap;
    private float[,]    _deltas;

    private Vector3[]   _lastVertices;
    private MeshFilter  _meshFilter;

    [ContextMenu("Create mesh")]
    void Create()
    {
        var mf = GetComponent<MeshFilter>();
        mf.sharedMesh = new Mesh();

        var vertices = new List<Vector3>();
        var tris = new List<int>();

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                vertices.Add(new Vector3(i * Unit, 0, j * Unit));

        for (int i = 0; i < Size - 1; i++)
        {
            for (int j = 0; j < Size - 1; j++)
            {
                tris.Add(i * Size + j);
                tris.Add(i * Size + j + 1);
                tris.Add((i + 1) * Size + j);

                tris.Add(i * Size + j + 1);
                tris.Add((i + 1) * Size + j + 1);
                tris.Add((i + 1) * Size + j);
            }
        }

        mf.sharedMesh.vertices = vertices.ToArray();
        mf.sharedMesh.triangles = tris.ToArray();

        mf.sharedMesh.RecalculateBounds();
        mf.sharedMesh.RecalculateNormals();
    }

    void Awake()
    {
        base.Awake();

        _meshFilter = GetComponent<MeshFilter>();
        _lastVertices = _meshFilter.mesh.vertices;
        _velocityMap = new float[Size * Size];
        _accelerationMap = new float[Size * Size];
        _deltas = new float[Size * Size, 4];
    }

    public float GetHeight(int x, int y)
    {
        return _lastVertices[y * Size + x].y;
    }

    public void Impulse(Vector3 point, float verticalVelocity)
    {
        var vertices = _meshFilter.mesh.vertices;

        float dst = float.MaxValue;
        int minIndex = -1;

        for (int i = 0; i < vertices.Length; i++)
        {
            var vertex = vertices[i];
            var fp = transform.TransformPoint(vertex);
            var sqrDst = Vector3.SqrMagnitude(fp - new Vector3(point.x, fp.y, point.z));
            if (sqrDst < dst)
            {
                dst = sqrDst;
                minIndex = i;
            }
        }
        vertices[minIndex] += new Vector3(0, verticalVelocity, 0);
        _meshFilter.mesh.vertices = vertices;
    }

    public void UpdateVertexMap()
    {
        _meshFilter.mesh.vertices = _lastVertices;
    }

    public void Impulse(int x, int y, float verticalVelocity)
    {
        if (x <= 0 || y <= 0 || x >= Size - 1 || y >= Size - 1) return;
        _lastVertices[y * Size + x] += new Vector3(0, verticalVelocity, 0);
    }

    public void Flatten(int x, int y, float verticalValue)
    {
        if (x <= 0 || y <= 0 || x >= Size - 1 || y >= Size - 1) return;
        _lastVertices[y * Size + x].y = verticalValue;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
            Impulse(new Vector3(10, 0, 10), -1);

        _lastVertices = _meshFilter.mesh.vertices;

        for (int i = 0; i < Size; i++)
            for (int j = 0; j < Size; j++)
                UpdateCell(i, j);

        _meshFilter.mesh.vertices = _lastVertices;
        _meshFilter.mesh.RecalculateBounds();
        _meshFilter.mesh.RecalculateNormals();
    }

    void UpdateCell(int i, int j)
    {
        int idx = j * Size + i;

        float force = SpringConstant * _lastVertices[idx].y + _velocityMap[idx] * Damping;
        _accelerationMap[idx] = -force;
        var res = _velocityMap[idx];
        _velocityMap[idx] += _accelerationMap[idx];

        _lastVertices[idx] += Vector3.up * res;

        _deltas[idx, 0] = Spread * (_lastVertices[idx].y - (i == Size - 1 ? 0 : _lastVertices[idx + 1].y));
        _deltas[idx, 1] = Spread * (_lastVertices[idx].y - (j == Size - 1 ? 0 : _lastVertices[(j + 1) * Size + i].y));
        _deltas[idx, 2] = Spread * (_lastVertices[idx].y - (i == 0 ? 0 : _lastVertices[idx - 1].y));
        _deltas[idx, 3] = Spread * (_lastVertices[idx].y - (j == 0 ? 0 : _lastVertices[(j - 1) * Size + i].y));

        if (i != Size -1)
            _velocityMap[idx + 1] += _deltas[idx, 0];
        if (j != Size - 1)
            _velocityMap[(j + 1) * Size + i] += _deltas[idx, 1];
        if (i > 0)
            _velocityMap[idx - 1] += _deltas[idx, 2];
        if (j > 0)
            _velocityMap[(j - 1) * Size + i] += _deltas[idx, 3];
    }
}
