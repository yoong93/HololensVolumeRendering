//Author and copyright owner: Matrix Inception Inc.
//Date: 2016-10-31
//This script controls higher level functions of the keyboard, namely Shift, Show / Hide, and Move / Pin.

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Windows.Speech;
using HoloToolkit.Sharing;


public class KeyboardMain : MonoBehaviour {

    public GameObject InputDisplay;
    public bool ShiftOn;
    public bool IsDone;
    public GameObject keyboardUpper;
    public GameObject keyboardLower;
    public GameObject keyboardSet;
    public GameObject keyDone;
    public bool IsMoving;
    public AudioClip[] keySounds;
    private bool started = false;


    // Use this for initialization    
    void Start()
    {
        keyboardUpper.SetActive(ShiftOn);
        keyboardLower.SetActive(!ShiftOn);
        keyboardSet.SetActive(!IsDone);
    }
    // Update is called once per frame
    void Update()
    {
        if (IsMoving) {
            transform.position = Vector3.Lerp(transform.position, Camera.main.transform.position + Camera.main.transform.forward * 1.5f, 0.1f );
            transform.LookAt(Camera.main.transform.position+ Camera.main.transform.forward * 4+ transform.right * (-0.39f)); 
        }
    }

    public bool getStarted()
    {
        return started;
    }

    public void setStarted()
    {
        started = true;
    }


    public void OnShift()
    {
        ShiftOn = !ShiftOn;
        keyboardUpper.SetActive(ShiftOn);
        keyboardLower.SetActive(!ShiftOn);
    }

    //The green square is the "Done" key, and it's kept as a separate key from the rest of the keyboard. 
    //Once selected it shows or hides the keyboard. Additional scripts can be added here to submit the message.
    public void OnDone()
    {
        IsDone = !IsDone;
        keyboardSet.SetActive(!IsDone);
        if (IsDone) {
            keyDone.transform.position += keyDone.transform.right * (-(0.726f+0.06f)) + keyDone.transform.up * (0.245f+0.1f);
        }
        else
        {
            keyDone.transform.position += keyDone.transform.right * (0.726f + 0.06f) + keyDone.transform.up * (-(0.245f + 0.1f));
        }
        GameObject sharing = GameObject.Find("Sharing");        
        sharing.GetComponent<SharingStage>().ServerAddress = InputDisplay.GetComponent<TextMesh>().text;
        sharing.GetComponent<SharingStage>().startSharing();
        GameObject.Find("HologramCollection").GetComponent<AppStateManager>().CurrentAppState = AppStateManager.AppState.PickingAvatar;
        CustomMessages.Instance.InitializeMessageHandlers();
        ImportExportAnchorManager.Instance.AnchorManagerStart();
        foreach (Transform child in this.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        Destroy(this);
    }    
}
