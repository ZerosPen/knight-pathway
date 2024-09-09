using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CammeraFollow : MonoBehaviour
{
    [SerializeField]
    private float speedFollow = 2f;
    [SerializeField] private Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPost = new Vector3(target.position.x, target.position.y, -10f);
        transform.position = Vector3.Slerp(transform.position,newPost, speedFollow * Time.deltaTime);
    }
}
