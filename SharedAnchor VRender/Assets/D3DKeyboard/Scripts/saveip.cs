using UnityEngine;
using System.Collections;

public class saveip : MonoBehaviour {
    private string ip;
    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
	
    public string getIP()
    {
        return ip;
    }
    public void setIP(string ipadd)
    {        
        ip = ipadd;
        Debug.Log(ip);
    }	
}
