using System.Collections;
using UnityEngine;

public class ItemManager : MonoBehaviourSingleton<ItemManager>
{
    public GameObject ItemPrefab;

    public float SpawningHeight = 25;
    public float DropTime = 5;
    public float ImpulseVelocity = -5;

    private GameObject _itemInstance;
    private bool _dropped;

    private Vector3 _targetPosition;
    private Vector3 TargetPosition
    {
        get
        {
            var x = Mathf.RoundToInt(_itemInstance.transform.position.x / GraphicMeshUpdater.Instance.Unit);
            var y = Mathf.RoundToInt(_itemInstance.transform.position.z / GraphicMeshUpdater.Instance.Unit);
            var h = GraphicMeshUpdater.Instance.GetHeight(x, y);
            _targetPosition.y = h;
            return _targetPosition;
        }

        set { _targetPosition = value; }
    }

    void Update()
    {
        if (null == _itemInstance)
            SpawnItem();
        if (_dropped && _itemInstance != null)
            UpdateItemPosition();
    }

    void SpawnItem()
    {
        Vector3 pos = new Vector3(Random.Range(1, Mathf.RoundToInt(GraphicMeshUpdater.Instance.Size * GraphicMeshUpdater.Instance.Unit) - 1),
                                  0,
                                  Random.Range(1, Mathf.RoundToInt(GraphicMeshUpdater.Instance.Size * GraphicMeshUpdater.Instance.Unit) - 1));
        _itemInstance = Instantiate(ItemPrefab, pos + Vector3.up * SpawningHeight, Quaternion.identity);
        _dropped = false;
        StartCoroutine(AsyncDropAnimation());
    }

    IEnumerator AsyncDropAnimation()
    {
        var initPosition = _itemInstance.transform.position;
        TargetPosition = initPosition;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / DropTime;
            yield return new WaitForEndOfFrame();
            _itemInstance.transform.position = Vector3.Lerp(initPosition, TargetPosition, t);
        }
        var x = Mathf.RoundToInt(TargetPosition.x / GraphicMeshUpdater.Instance.Unit);
        var y = Mathf.RoundToInt(TargetPosition.z / GraphicMeshUpdater.Instance.Unit);

        GraphicMeshUpdater.Instance.Impulse(y, x, ImpulseVelocity);
        _dropped = true;
    }

    public void CollectItem(Vector3 pos)
    {
        if (Vector3.Distance(pos, _itemInstance.transform.position) < GraphicMeshUpdater.Instance.Unit)
        {
            Snake.Instance.IncrementSize = true;
            Destroy(_itemInstance);
        }
    }

    public void UpdateItemPosition()
    {
        _itemInstance.transform.position = TargetPosition;
    }
}
