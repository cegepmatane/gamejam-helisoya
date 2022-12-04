using UnityEngine;
using Mirror;

public class AmmoBox : NetworkBehaviour {
    public float amplitude;          //Set in Inspector 
    public float speed;                  //Set in Inspector 
    private float _tempVal;
    private Vector3 _tempPos;
    [SerializeField] private int ammoNumber = 20;


    // Start is called before the first frame update
    void Start() {
        _tempPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        _tempPos.y = amplitude * Mathf.Sin(speed * Time.time);
        transform.localPosition = _tempPos;
    }


    public int GetAmmo()
    {
        return ammoNumber;
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player") && collision.GetComponent<HunterMovement>().isLocalPlayer)
        {
            HunterMovement.localPlayer.AddAmmoFromBox(this);
        }
    }


}
