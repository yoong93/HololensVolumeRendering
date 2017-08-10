using HoloToolkit;
using HoloToolkit.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using Vuforia;

public class KeyWordRecognizer : MonoBehaviour
{

    // KeywordRecognizer object.
    public AudioClip keywordClip;
    public AudioSource keywordSource;
    KeywordRecognizer keywordRecognizer;
    // Defines which function to call when a keyword is recognized.
    delegate void KeywordAction(PhraseRecognizedEventArgs args);
    Dictionary<string, KeywordAction> keywordCollection;


    void Start()
    {

        keywordCollection = new Dictionary<string, KeywordAction>();
        keywordCollection.Add("Scale model", scaleModelCommand);
        /* TODO: DEVELOPER CODING EXERCISE 5.a */

        // 5.a: Add keyword Expand Model to call the ExpandModelCommand function.
        keywordCollection.Add("Enable Model", enableModelCommand);
        keywordCollection.Add("Move Model", moveModelCommand);
        keywordCollection.Add("Reset Model", resetModelCommand);
        keywordCollection.Add("Rotate Model", turnModelCommand);
        keywordCollection.Add("drag Model", dragModelCommand);


        keywordCollection.Add("move right", addXCommand);
        keywordCollection.Add("move left", minusXCommand);
        keywordCollection.Add("move up", addYCommand);
        keywordCollection.Add("move down", minusYCommand);
        keywordCollection.Add("move front", addZCommand);
        keywordCollection.Add("move back", minusZCommand);

        keywordCollection.Add("Start Tracking", startTrackingCommand);
        



        // Initialize KeywordRecognizer with the previously added keywords.
        keywordRecognizer = new KeywordRecognizer(keywordCollection.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();      

    }

    void OnDestroy()
    {
        keywordRecognizer.Dispose();
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        KeywordAction keywordAction;
        if (keywordCollection.TryGetValue(args.text, out keywordAction))
        {
            keywordAction.Invoke(args);
            keywordSource.PlayOneShot(keywordClip);
            
        }
    }

    private void startTrackingCommand(PhraseRecognizedEventArgs args)
    {
        GestureManager.Instance.Transition(GestureManager.Instance.gestureRecognizer);
        HologramPlacement.Instance.GotTransform = false;
        HologramPlacement.Instance.DisableModel();
        GameObject.Find("ImageTarget").GetComponent<DefaultTrackableEventHandler>().active = true;   
    }

    private void addXCommand(PhraseRecognizedEventArgs args)
    {
        HologramPlacement.Instance.adjustPosition += new Vector3(0.1f, 0.0f, 0.0f);
        RemotePlayerManager.Instance.adjustPosition += new Vector3(0.1f, 0.0f, 0.0f);
    }

    private void addYCommand(PhraseRecognizedEventArgs args)
    {
        HologramPlacement.Instance.adjustPosition += new Vector3(0.0f, 0.1f, 0.0f);
        RemotePlayerManager.Instance.adjustPosition += new Vector3(0.0f, 0.1f, 0.0f);
    }

    private void addZCommand(PhraseRecognizedEventArgs args)
    {
        HologramPlacement.Instance.adjustPosition += new Vector3(0.0f, 0.0f, 0.1f);
        RemotePlayerManager.Instance.adjustPosition += new Vector3(0.0f, 0.0f, 0.1f);
    }

    private void minusXCommand(PhraseRecognizedEventArgs args)
    {
        HologramPlacement.Instance.adjustPosition -= new Vector3(0.1f, 0.0f, 0.0f);
        RemotePlayerManager.Instance.adjustPosition -= new Vector3(0.1f, 0.0f, 0.0f);
    }

    private void minusYCommand(PhraseRecognizedEventArgs args)
    {
        HologramPlacement.Instance.adjustPosition -= new Vector3(0.0f, 0.1f, 0.0f);
        RemotePlayerManager.Instance.adjustPosition -= new Vector3(0.0f, 0.1f, 0.0f);
    }

    private void minusZCommand(PhraseRecognizedEventArgs args)
    {
        HologramPlacement.Instance.adjustPosition -= new Vector3(0.0f, 0.0f, 0.1f);
        RemotePlayerManager.Instance.adjustPosition -= new Vector3(0.0f, 0.0f, 0.1f);
    }

    private void scaleModelCommand(PhraseRecognizedEventArgs args)
    {
        if (HologramPlacement.Instance.GotTransform == true)
        {
            GestureManager.Instance.Transition(GestureManager.Instance.scaleRecognizer);
        }
    }

    private void dragModelCommand(PhraseRecognizedEventArgs args)
    {
        if (HologramPlacement.Instance.GotTransform == true)
        {
            GestureManager.Instance.Transition(GestureManager.Instance.dragRecognizer);
            HologramPlacement.Instance.initPos = HologramPlacement.Instance.transform.position;            
        }
    }

    private void turnModelCommand(PhraseRecognizedEventArgs args)
    {
        if (HologramPlacement.Instance.GotTransform == true)
        {
            GestureManager.Instance.Transition(GestureManager.Instance.turnRecognizer);
        }
    }

    private void resetModelCommand(PhraseRecognizedEventArgs args)
    {
        HologramPlacement.Instance.SendMessage("ResetStage");
        GestureManager.Instance.Transition(GestureManager.Instance.gestureRecognizer);
    }

    private void enableModelCommand(PhraseRecognizedEventArgs args)
    {
        
        if (HologramPlacement.Instance.render == false || GameObject.Find("ImageTarget").GetComponent<DefaultTrackableEventHandler>().active == true)
        {
            GameObject.Find("ImageTarget").GetComponent<DefaultTrackableEventHandler>().active = false;
            HologramPlacement.Instance.moveMag = 3.0f;            
            this.moveModelCommand(args);
            HologramPlacement.Instance.EnableModel();      
        }

    }

    private void moveModelCommand(PhraseRecognizedEventArgs args)
    {
        if (HologramPlacement.Instance.render)
        {
            HologramPlacement.Instance.MoveModel();
            GestureManager.Instance.Transition(GestureManager.Instance.gestureRecognizer);
            HologramPlacement.Instance.moveMag = Vector3.Magnitude(HologramPlacement.Instance.transform.position - Camera.main.transform.position);
        }
        
    }

}