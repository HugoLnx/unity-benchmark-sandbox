using UnityEngine;

public class NodeBuilder
{
    public int TotalGenerated { get; private set; }
    private readonly float _positionNoiseRate;
    private readonly float _rotationNoiseRate;
    private readonly float _scaleNoiseRate;
    private readonly string _nodeTag;

    public NodeBuilder(float positionNoiseRate, float rotationNoiseRate, float scaleNoiseRate, string tag)
    {
        _positionNoiseRate = positionNoiseRate;
        _rotationNoiseRate = rotationNoiseRate;
        _scaleNoiseRate = scaleNoiseRate;
        _nodeTag = tag;
    }

    public GameObject Build(GameObject prefab, Transform parent = null)
    {
        GameObject obj = prefab == null
            ? new GameObject()
            : GameObject.Instantiate(prefab);
        obj.name = $"Node#{++TotalGenerated}";
        if (parent != null) obj.transform.SetParent(parent);
        obj.tag = _nodeTag;
        if (ShouldApplyNoise(_positionNoiseRate))
        {
            ApplyPositionNoise(obj);
        }
        if (ShouldApplyNoise(_rotationNoiseRate))
        {
            ApplyRotationNoise(obj);
        }
        if (ShouldApplyNoise(_scaleNoiseRate))
        {
            ApplyScaleNoise(obj);
        }
        return obj;
    }

    private void ApplyPositionNoise(GameObject obj)
    {
        obj.transform.localPosition = UnityEngine.Random.insideUnitSphere;
    }

    private void ApplyRotationNoise(GameObject obj)
    {
        obj.transform.localRotation = Quaternion.Euler(UnityEngine.Random.insideUnitSphere * 360f);
    }

    private void ApplyScaleNoise(GameObject obj)
    {
        obj.transform.localScale = UnityEngine.Random.insideUnitSphere;
    }

    private bool ShouldApplyNoise(float noiseRate)
    {
        if (noiseRate <= 0f) return false;
        return TotalGenerated % (int)(1f / noiseRate) == 0;
    }
}
