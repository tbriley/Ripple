using System.Collections;
using UnityEngine;

public class ItemManager : MonoBehaviourSingleton<ItemManager>
{
    public GameObject ItemPrefab;

    public float SpawningHeight = 25;
    public float DropTime = 5;
    public float ImpulseVelocity = -5;

    private GameObject _itemInstance;

    void Update()
    {
        if (null == _itemInstance)
            SpawnItem();
        UpdateItemPosition();
    }

    void SpawnItem()
    {
        Vector3 pos = new Vector3(Random.Range(0, Mathf.RoundToInt(GraphicMeshUpdater.Instance.Size * GraphicMeshUpdater.Instance.Unit)),
                                  0,
                                  Random.Range(0, Mathf.RoundToInt(GraphicMeshUpdater.Instance.Size * GraphicMeshUpdater.Instance.Unit)));
        _itemInstance = Instantiate(ItemPrefab, pos + Vector3.up * SpawningHeight, Quaternion.identity);
        StartCoroutine(AsyncDropAnimation());
    }

    IEnumerator AsyncDropAnimation()
    {
        var initPosition = _itemInstance.transform.position;
        var targetPosition = initPosition;
        targetPosition.y = 0;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / DropTime;
            yield return new WaitForEndOfFrame();
            _itemInstance.transform.position = Vector3.Lerp(initPosition, targetPosition, t);
            GraphicMeshUpdater.Instance.Impulse(targetPosition, -1);
        }
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
        
    }
}
