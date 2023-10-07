using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float velocidadeGiro = 3.0f;
    private Vector3 pos;
    private Transform inimigo;
    private bool achou = false;

    // Start is called before the first frame update
    void Start()
    {
        inimigo = GameObject.FindGameObjectWithTag("Inimigo").transform;
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position;
        pos.x += moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        pos.z += moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position = pos;

        if( achou )
        {
            Quaternion targetRotation = Quaternion.LookRotation(inimigo.transform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, velocidadeGiro * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider colisao)
    {
        if( colisao.gameObject.name == "Inimigo")
        {
            achou = true;
        }
        
    }

    private void OnTriggerExit(Collider colisao)
    {
        if( colisao.gameObject.name == "Inimigo")
        {
            achou = false;
        }
        
    }
}
