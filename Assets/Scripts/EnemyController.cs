using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyType
    {
        EnemyLittle,
        EnemyLevel1,
        EnemyLevel2,
        EnemyLevel3
    };
    public EnemyType enemyType;
    public GameObject dieParticlePrefab;
    public float allBetweenDelay = 0.4f;
    public float allAutoReleaseTime = 4.0f;
    private Transform nameText;
    private TextMeshPro textMeshPro;
    private bool colliderAdded = false;
    private MeshRenderer textmeshproRenderer;
    

    // Start is called before the first frame update
    void Awake()
    {
        nameText = Utils.FindChildByName(gameObject, "NameText");
        textMeshPro = nameText.GetComponent<TextMeshPro>();
        textmeshproRenderer = textMeshPro.GetComponent<MeshRenderer>();

        CustomBulletShot[] shots = GetComponentsInChildren<CustomBulletShot>();
        foreach(var shot in shots)
        {
            shot._BetweenDelay = allBetweenDelay;
            shot._AutoReleaseTime = allAutoReleaseTime;
        }
    }

    void Start() {
        Health health = GetComponent<Health>();
        if(health)
        {
            health.OnDeath += OnDeath;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(!colliderAdded)
        {
            colliderAdded = TryAddCollider();
        }
        
    }

    private bool TryAddCollider()
    {
        Bounds bounds = textmeshproRenderer.bounds;
        if(bounds.extents.x > 0 && bounds.extents.y > 0)
        {
            BoxCollider boxCollider = nameText.gameObject.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(bounds.extents.x * 2, bounds.extents.y * 2, 0.5f);
            return true;
        }
        return false;
    }

    public void SetDisplayName(string name)
    {
        textMeshPro.text = name;
    }

    public void SetDisplayColor(Color color)
    {
        textMeshPro.color = color;
    }

    private void OnDeath()
    {
        if(dieParticlePrefab)
        {
            Instantiate(dieParticlePrefab, this.gameObject.transform.position, Quaternion.identity);
        }
    }
}
