using UnityEngine;

public class UnfateShadow : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;
    
    void Update()
    {
        if (target == null)
            Destroy(gameObject);
        else
        {
            transform.localScale = new Vector3(Mathf.Sign(target.localScale.x), 1, 1);
            var distance = Vector3.Distance(transform.position, target.position);
            transform.position = Vector3.MoveTowards(transform.position, target.position, distance * speed * Time.deltaTime);
        }
    }
}
