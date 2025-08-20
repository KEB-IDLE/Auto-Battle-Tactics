using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class to display the HealthBar and FloatingText Canvas UI in WorldSpace
/// </summary>
public class WorldCanvasController : MonoBehaviour
{
    public GameObject worldCanvas;
    public GameObject floatingTextPrefab;
    public GameObject healthBarPrefab;

    public void AddDamageText(Vector3 position, float v)
    {
        if (!worldCanvas || !floatingTextPrefab)
        {
            Debug.LogError("[WorldCanvasController] worldCanvas/floatingTextPrefab not set");
            return;
        }

        var go = Instantiate(floatingTextPrefab);
        go.transform.SetParent(worldCanvas.transform, false); // ★
        go.transform.localScale = Vector3.one;                // ★

        var ft = go.GetComponent<FloatingText>();
        if (ft) ft.Init(position, v);
        else Debug.LogError("[WorldCanvasController] FloatingText component missing on prefab");
    }

    public GameObject AddHealthBar(GameObject championGO)
    {
        if (!worldCanvas || !healthBarPrefab)
        {
            Debug.LogError("[WorldCanvasController] worldCanvas/healthBarPrefab not set");
            return null;
        }

        var go = Instantiate(healthBarPrefab);
        go.transform.SetParent(worldCanvas.transform, false); // ★
        go.transform.localScale = Vector3.one;                // ★

        var hb = go.GetComponent<HealthBar>();
        if (hb) hb.Init(championGO);
        else Debug.LogError("[WorldCanvasController] HealthBar component missing on prefab");

        return go;
    }
}
