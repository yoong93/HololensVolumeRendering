using UnityEngine;
using HoloToolkit.Sharing;

using HoloToolkit.Unity;



/// <summary>

/// Keeps track of the current state of the experience.

/// </summary>

public class AppStateManager : Singleton<AppStateManager>

{

    /// <summary>

    /// Enum to track progress through the experience.

    /// </summary>    
    public enum AppState

    {

        Starting = 0,

        EnterIp,

        PickingAvatar,

        WaitingForAnchor,

        Ready

    }



    // The object to call to make a projectile.

    GameObject shootHandler = null;



    /// <summary>

    /// Tracks the current state in the experience.

    /// </summary>

    public AppState CurrentAppState { get; set; }



    void Start()

    {
        
        // The shootHandler shoots projectiles.

        if (GetComponent<ProjectileLauncher>() != null)

        {

            shootHandler = GetComponent<ProjectileLauncher>().gameObject;

        }



        // We start in the 'picking avatar' mode.

        CurrentAppState = AppState.EnterIp;



        
        // On device we start by showing the avatar picker.

        //PlayerAvatarStore.Instance.SpawnAvatarPicker();

    }



    public void ResetStage()

    {

        // If we fall back to waiting for anchor, everything needed to 

        // get us into setting the target transform state will be setup.

        if (CurrentAppState != AppState.PickingAvatar)

        {

            CurrentAppState = AppState.WaitingForAnchor;

        }



        // Reset the underworld.

        if (UnderworldBase.Instance)

        {

            UnderworldBase.Instance.ResetUnderworld();

        }

    }



    void Update()

    {
        switch (CurrentAppState)

        {
            case AppState.EnterIp:
                break;

            case AppState.PickingAvatar:
                if(PlayerAvatarStore.Instance.pickerSpawned == false)
                {
                    PlayerAvatarStore.Instance.SpawnAvatarPicker();
                    PlayerAvatarStore.Instance.pickerSpawned = true;
                }
                // Avatar picking is done when the avatar picker has been dismissed.

                if (PlayerAvatarStore.Instance.PickerActive == false)

                {

                    CurrentAppState = AppState.WaitingForAnchor;

                }

                break;

            case AppState.WaitingForAnchor:

                // Once the anchor is established we need to run spatial mapping for a 

                // little while to build up some meshes.

                if (ImportExportAnchorManager.Instance.AnchorEstablished)

                {

                    CurrentAppState = AppState.Ready;

                    GameObject.Find("DataCube").GetComponent<Loader>().StartVRendering();
                    GestureManager.Instance.OverrideFocusedObject = HologramPlacement.Instance.gameObject;


                }

                break;
        }

    }

}