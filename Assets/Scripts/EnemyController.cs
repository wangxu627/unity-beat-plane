using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform nameText;
    private TextMeshPro textMeshPro;
    private bool colliderAdded = false;
    private MeshRenderer textmeshproRenderer;
    // Start is called before the first frame update
    void Start()
    {
        this.nameText = Utils.FindChildByName(this.gameObject, "NameText");
        this.textMeshPro = this.nameText.GetComponent<TextMeshPro>();
        this.textmeshproRenderer = this.textMeshPro.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!this.colliderAdded)
        {
            this.colliderAdded = this.TryAddCollider();
        }
        
    }

    private bool TryAddCollider()
    {
        Bounds bounds = this.textmeshproRenderer.bounds;
        if(bounds.extents.x > 0 && bounds.extents.y > 0)
        {
            BoxCollider boxCollider = nameText.gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(bounds.extents.x * 2, bounds.extents.y * 2, 0);
            return true;
        }
        return false;
    }
}
