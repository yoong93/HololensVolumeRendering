using UnityEngine;

using System.Collections.Generic;

using UnityEngine.Windows.Speech;

using HoloToolkit.Unity;

using HoloToolkit.Sharing;

using Vuforia;



public class HologramPlacement : Singleton<HologramPlacement>

{

    /// <summary>

    /// Tracks if we have been sent a transform for the model.

    /// The model is rendered relative to the actual anchor.

    /// </summary>
    public bool GotTransform { get; set; }
    public bool render { get; set; }
    //private bool moving;
    
    public float scale_sensitivity;
    public float rotate_sensitivity;    
    private Vector3 scale;
    private Quaternion rotate;
    private bool host = true;
    private bool hostchecked = false; 
    public Vector3 adjustPosition;
    public Vector3 initPos;
    public float moveMag;
    /// <summary>

    /// When the experience starts, we disable all of the rendering of the model.

    /// </summary>

    List<MeshRenderer> disabledRenderers = new List<MeshRenderer>();






    void Start()

    {

        // When we first start, we need to disable the model to avoid it obstructing the user picking a hat.
        adjustPosition = new Vector3(0.0f, 0.0f, 0.0f);
        DisableModel();
        scale = transform.localScale;
        rotate = transform.localRotation;
        // We care about getting updates for the model transform.

        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.StageTransform] = this.OnStageTransfrom;
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.Turn] = this.OnTurnTransfrom;

        // And when a new user join we will send the model transform we have.
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
    }


    private void checkHost()
    {
        long id = SharingStage.Instance.Manager.GetLocalUser().GetID();
        int index = SharingSessionTracker.Instance.UserIds.IndexOf(id);
        if (index != -1)            {
                if (index == 0) { host = true; }
                else { host = false; }
                hostchecked = true;
       }
            else
            {
                hostchecked = false;
                Destroy(GetComponent<GestureManager>());
                Destroy(GetComponent<KeyWordRecognizer>());
            }
    }    
    public void ResetStage()

    {
        if (host)
        {
            GotTransform = false;
            transform.localScale = scale;
            transform.localRotation = rotate;
            AppStateManager.Instance.ResetStage();
        }
    }

    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)

    {
        if (host)
        {
            CustomMessages.Instance.SendStageTransform(transform.localPosition, transform.localRotation, transform.localScale, render);
        }
        
    }
    
    private Vector3 CalculateCenter()
    {
        Vector3 center = new Vector3(0.0f, 0.0f, 0.0f);
        float count = 0.0f;
        foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())
        {
            if (renderer.enabled)
            {
                center += renderer.bounds.center;
                count += 1.0f;
            }
        }

        center /= count;
        return center;
    }
    
    public void DisableModel()

    {

        foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())

        {

            if (renderer.enabled)

            {

                renderer.enabled = false;

                disabledRenderers.Add(renderer);

            }

        }



        foreach (MeshCollider collider in gameObject.GetComponentsInChildren<MeshCollider>())

        {

            collider.enabled = false;

        }
        render = false;
    }

    public void EnableModel()

    {
        Debug.Log("Enable called");
        foreach (MeshRenderer renderer in gameObject.GetComponentsInChildren<MeshRenderer>())

        {

            renderer.enabled = true;

        }



        foreach (MeshCollider collider in gameObject.GetComponentsInChildren<MeshCollider>())

        {

            collider.enabled = true;

        }
        disabledRenderers.Clear();
        render = true;
    }


    void Update()

    {
        //first check if host is confirmed
        if (!hostchecked && AppStateManager.Instance.CurrentAppState != AppStateManager.AppState.EnterIp)
        {
            checkHost();
        }else
        {
            if (host) { 
                if (GotTransform == false && GameObject.Find("ImageTarget").GetComponent<DefaultTrackableEventHandler>().active == false)
                {
                    transform.position = Vector3.Lerp(transform.position, ProposeTransformPosition(), 0.2f);
                }
                else {
                    if (GotTransform == true && GestureManager.Instance.scaling == true)
                    {
                        PerformScale();
                    }
                    if (GotTransform == true && GestureManager.Instance.turning == true)
                    {
                        PerformTurn();
                    }
                    if(GotTransform == true && GestureManager.Instance.dragging == true)
                    {
                        PerformDrag();
                    }
                }
                //CustomMessages.Instance.SendStageTransform(transform.localPosition, transform.localRotation, transform.localScale, render);
            }
            else
                {
                    //Debug.Log("adjust : " + adjustPosition);
                }
        }
    }
    
    private void PerformDrag()
    {
        float factor = 1.0f;
        if (GestureManager.Instance.dragPosition.y < 0)
        {
            factor = -1.0f;
        }
        Vector3 relativePos = Camera.main.transform.position - initPos;
        while (Vector3.Magnitude(relativePos) < 1.0f)
        {
            relativePos *= 3.0f;
        }
        Vector3 normalized = relativePos.normalized;
        normalized /= 20.0f;
        
        if (factor < 0 || (factor > 0 && Vector3.Magnitude(Camera.main.transform.position - this.transform.position) > 0.5f)){
            this.transform.position += factor * normalized;
        }
    }
    private void PerformScale()
    {
        float scaleFactor;
        scaleFactor = GestureManager.Instance.scalePosition.y * scale_sensitivity;
        transform.localScale += new Vector3(scaleFactor, scaleFactor, scaleFactor);

        if (transform.localScale.x < 0.1)
        {
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }

        //CustomMessages.Instance.SendScale(transform.localScale);
    }

    private void PerformTurn()
    {
        Vector3 center = CalculateCenter();
        float rotationFactor;
        Vector3 navigationVector = GestureManager.Instance.turnPosition;
        rotationFactor = navigationVector.magnitude * rotate_sensitivity;
        float zfactor;
        if (navigationVector.y > 0)
        {
            zfactor = -0.5f;
        }
        else
        {
            zfactor = 0.5f;
        }

        Vector3 axis = -new Vector3(-navigationVector.y, navigationVector.x, zfactor);
        transform.RotateAround(center, axis, rotationFactor);
        
    }

    public void OnSelect()
    {
        if(AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.Ready)
        {
            GotTransform = true;            
            GameObject.Find("ImageTarget").GetComponent<DefaultTrackableEventHandler>().active = false;
            GestureManager.Instance.Transition(GestureManager.Instance.setRecognizer);
        }
    }


    Vector3 ProposeTransformPosition()

    {
        return Camera.main.transform.position + Camera.main.transform.forward * moveMag;
    }

    void OnScaleTransfrom(NetworkInMessage msg)

    {
        transform.localScale = CustomMessages.Instance.ReadVector3(msg);
    }

    public bool getHostStatus()
    {
        return host;
    }
    void OnStageTransfrom(NetworkInMessage msg)

    {
        msg.ReadInt64();
        transform.localPosition = CustomMessages.Instance.ReadVector3(msg) + adjustPosition;
        transform.localRotation = CustomMessages.Instance.ReadQuaternion(msg);
        transform.localScale = CustomMessages.Instance.ReadVector3(msg);        
        float renderGot = msg.ReadFloat();
        bool statusgot = false;
        if(renderGot == 0.0f)
        {
            statusgot = false;
            if(render == true && statusgot == false)
            {
                DisableModel();
            }
            
        }else if(renderGot == 1.0f)
        {
            statusgot = true;
            if (render == false && statusgot == true)
            {
                EnableModel();
            }
        }
        else
        {
            Debug.Log("RENDER STATUS ERROR");
        }
    }

    void OnTurnTransfrom(NetworkInMessage msg)

    {
        transform.localRotation = CustomMessages.Instance.ReadQuaternion(msg);
    }



    /// <summary>

    /// When a remote system has a transform for us, we'll get it here.

    /// </summary>

    void OnResetStage(NetworkInMessage msg)
    {
        GotTransform = false;
        AppStateManager.Instance.ResetStage();
    }

    public void MoveModel()
    {
        GotTransform = false;
        transform.position = Vector3.Lerp(transform.position, ProposeTransformPosition(), 0.2f);
    }
}
