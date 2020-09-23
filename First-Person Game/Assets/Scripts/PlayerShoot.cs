using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    //variables needed to spawn the bullet
    public GameObject bullet;
    Vector3 spawnPoint;
    Vector3 rotation;
    [SerializeField] private float offset = default;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnPoint = transform.position + transform.forward * offset;
        rotation = bullet.transform.rotation.eulerAngles;
        ShootEvent();
    }

    private void ShootEvent()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Object.Instantiate(bullet, spawnPoint, Quaternion.Euler(rotation.x, transform.rotation.y, rotation.z));
        }
    }
}
