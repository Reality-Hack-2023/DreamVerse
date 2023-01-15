using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activateTunnels : MonoBehaviour
{
    public GameObject[] tunnels;
    private int count;
    private int index;
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        index = 0;
        counter();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(count);
        if (count == 1) {
            tunnels[0].SetActive(true);
            //tunnels[1].SetActive(false);
        } else if (count == 18) {
            tunnels[0].SetActive(false);
            tunnels[1].SetActive(true);
        }else if (count == 34) {
            tunnels[1].SetActive(false);
            tunnels[2].SetActive(true);
        } else if (count == 51) {
            tunnels[2].SetActive(false);
            tunnels[3].SetActive(true);
        }else if (count == 72) {
            tunnels[3].SetActive(false);
            tunnels[4].SetActive(true);
        } else if (count == 90) {
            tunnels[4].SetActive(false);
            tunnels[5].SetActive(true);
        }else if (count == 112) {
            tunnels[5].SetActive(false);
        } 


    }

    private void counter() {
        count+=1;
        Invoke("counter" , 1);
    }
}
