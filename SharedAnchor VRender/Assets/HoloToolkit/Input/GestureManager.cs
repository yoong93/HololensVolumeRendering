using UnityEngine;
using System.Collections;
using UnityEngine.VR.WSA.Input;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// GestureManager creates a gesture recognizer and signs up for a tap gesture.
    /// When a tap gesture is detected, GestureManager uses GazeManager to find the game object.
    /// GestureManager then sends a message to that game object.
    /// </summary>
    [RequireComponent(typeof(GazeManager))]
    public class GestureManager : Singleton<GestureManager>
    {
        /// <summary>
        /// To select even when a hologram is not being gazed at,
        /// set the override focused object.
        /// If its null, then the gazed at object will be selected.
        /// </summary>
        public GameObject OverrideFocusedObject
        {
            get; set;
        }

        public GestureRecognizer gestureRecognizer { get; private set; }
        public GestureRecognizer scaleRecognizer { get; private set; }
        public GestureRecognizer turnRecognizer { get; private set; }
        public GestureRecognizer setRecognizer { get; private set; }
        public GestureRecognizer dragRecognizer { get; private set; }


        public GestureRecognizer ActiveRecognizer { get; private set; }
        
        
        public bool scaling { get; private set; }
        public bool turning { get; private set; }
        public bool dragging { get; private set; }

        public Vector3 scalePosition { get; private set; }
        public Vector3 turnPosition { get; private set; }
        public Vector3 dragPosition { get; private set; }

        private GameObject focusedObject;


        void Start()
        {
            // Create a new GestureRecognizer. Sign up for tapped events.
            gestureRecognizer = new GestureRecognizer();
            gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);

            gestureRecognizer.TappedEvent += GestureRecognizer_TappedEvent;



            setRecognizer = new GestureRecognizer();

            turnRecognizer = new GestureRecognizer();
            turnRecognizer.SetRecognizableGestures(
                GestureSettings.Tap |
                GestureSettings.NavigationX |
                GestureSettings.NavigationY);

            turnRecognizer.NavigationStartedEvent += turnRecognizer_turnStartedEvent;
            turnRecognizer.NavigationUpdatedEvent += turnRecognizer_turnUpdatedEvent;
            turnRecognizer.NavigationCompletedEvent += turnRecognizer_turnCompletedEvent;
            turnRecognizer.NavigationCanceledEvent += turnRecognizer_turnCanceledEvent;            

            //scale Recognizer
            scaleRecognizer = new GestureRecognizer();
            scaleRecognizer.SetRecognizableGestures(
                GestureSettings.Tap |
                GestureSettings.NavigationY);

            //scaleRecognizer.TappedEvent += scaleRecognizer_TappedEvent;
            scaleRecognizer.NavigationStartedEvent += scaleRecognizer_scaleStartedEvent;
            scaleRecognizer.NavigationUpdatedEvent += scaleRecognizer_scaleUpdatedEvent;
            scaleRecognizer.NavigationCompletedEvent += scaleRecognizer_scaleCompletedEvent;
            scaleRecognizer.NavigationCanceledEvent += scaleRecognizer_scaleCanceledEvent;


            //drag recognizer
            dragRecognizer = new GestureRecognizer();
            dragRecognizer.SetRecognizableGestures(
                GestureSettings.Tap |
                GestureSettings.NavigationY);

            dragRecognizer.NavigationStartedEvent += dragRecognizer_dragStartedEvent;
            dragRecognizer.NavigationUpdatedEvent += dragRecognizer_dragUpdatedEvent;
            dragRecognizer.NavigationCompletedEvent += dragRecognizer_dragCompletedEvent;
            dragRecognizer.NavigationCanceledEvent += dragRecognizer_dragCanceledEvent;

            Transition(gestureRecognizer);

            // Start looking for gestures.
            //gestureRecognizer.StartCapturingGestures();
            //scaleRecognizer.StartCapturingGestures();
        }

        private void GestureRecognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
        {
            if (PlayerAvatarStore.Instance == null || PlayerAvatarStore.Instance.PickerActive || AppStateManager.Instance.CurrentAppState == AppStateManager.AppState.EnterIp)
            {
                if (focusedObject != null)
                {
                    focusedObject.SendMessage("OnSelect");
                }
            }
            else
            {   
                if (!PlayerAvatarStore.Instance.PickerActive)
                {
                    HologramPlacement.Instance.SendMessage("OnSelect");
                }
                
            }

        }
        void LateUpdate()
        {
            GameObject oldFocusedObject = focusedObject;

            if (GazeManager.Instance.Hit &&
                OverrideFocusedObject == null &&
                GazeManager.Instance.HitInfo.collider != null)
            {
                // If gaze hits a hologram, set the focused object to that game object.
                // Also if the caller has not decided to override the focused object.
                focusedObject = GazeManager.Instance.HitInfo.collider.gameObject;
            }
            else
            {
                // If our gaze doesn't hit a hologram, set the focused object to null or override focused object.
                focusedObject = OverrideFocusedObject;
            }

            if (focusedObject != oldFocusedObject)
            {
                // If the currently focused object doesn't match the old focused object, cancel the current gesture.
                // Start looking for new gestures.  This is to prevent applying gestures from one hologram to another.
                ActiveRecognizer.CancelGestures();
                ActiveRecognizer.StartCapturingGestures();
            }
        }

        void OnDestroy()
        {
            //gestureRecognizer.StopCapturingGestures();
            gestureRecognizer.TappedEvent -= GestureRecognizer_TappedEvent;


            //scaleRecognizer.TappedEvent -= scaleRecognizer_TappedEvent;
            scaleRecognizer.NavigationStartedEvent -= scaleRecognizer_scaleStartedEvent;
            scaleRecognizer.NavigationUpdatedEvent -= scaleRecognizer_scaleUpdatedEvent;
            scaleRecognizer.NavigationCompletedEvent -= scaleRecognizer_scaleCompletedEvent;
            scaleRecognizer.NavigationCanceledEvent -= scaleRecognizer_scaleCanceledEvent;

            turnRecognizer.NavigationStartedEvent -= turnRecognizer_turnStartedEvent;
            turnRecognizer.NavigationUpdatedEvent -= turnRecognizer_turnUpdatedEvent;
            turnRecognizer.NavigationCompletedEvent -= turnRecognizer_turnCompletedEvent;
            turnRecognizer.NavigationCanceledEvent -= turnRecognizer_turnCanceledEvent;

            dragRecognizer.NavigationStartedEvent -= dragRecognizer_dragStartedEvent;
            dragRecognizer.NavigationUpdatedEvent -= dragRecognizer_dragUpdatedEvent;
            dragRecognizer.NavigationCompletedEvent -= dragRecognizer_dragCompletedEvent;
            dragRecognizer.NavigationCanceledEvent -= dragRecognizer_dragCanceledEvent;

        }


        public void Transition(GestureRecognizer newRecognizer)
        {
            
            Debug.Log("newrecognizer : " +  newRecognizer + "recognizable : " + newRecognizer.GetRecognizableGestures());
            
            if (newRecognizer == null)
            {
                return;
            }

            if (ActiveRecognizer != null)
            {
                if (ActiveRecognizer == newRecognizer)
                {
                    return;
                }

                ActiveRecognizer.CancelGestures();
                ActiveRecognizer.StopCapturingGestures();
            }

            newRecognizer.StartCapturingGestures();
            ActiveRecognizer = newRecognizer;
        }
        private void scaleRecognizer_scaleStartedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            // 2.b: Set scaling to be true.
            scaling = true;            

            // 2.b: Set scalePosition to be relativePosition.
            scalePosition = relativePosition;
        }

        private void scaleRecognizer_scaleUpdatedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            // 2.b: Set scaling to be true.
            scaling = true;

            // 2.b: Set scalePosition to be relativePosition.
            scalePosition = relativePosition;
        }

        private void scaleRecognizer_scaleCompletedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            // 2.b: Set scaling to be false.            
            scaling = false;
        }

        private void scaleRecognizer_scaleCanceledEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            // 2.b: Set scaling to be false.
            scaling = false;
        }


        private void dragRecognizer_dragStartedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            // 2.b: Set scaling to be true.
            dragging = true;

            // 2.b: Set scalePosition to be relativePosition.
            dragPosition = relativePosition;
        }

        private void dragRecognizer_dragUpdatedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            // 2.b: Set scaling to be true.
            dragging = true;

            // 2.b: Set scalePosition to be relativePosition.
            dragPosition = relativePosition;
        }

        private void dragRecognizer_dragCompletedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            // 2.b: Set scaling to be false.            
            dragging = false;
        }

        private void dragRecognizer_dragCanceledEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            // 2.b: Set scaling to be false.
            dragging = false;
        }


        //Events for turnings
        private void turnRecognizer_turnStartedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            turning = true;
            turnPosition = relativePosition;
        }

        private void turnRecognizer_turnUpdatedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            turning = true;
            turnPosition = relativePosition;
        }

        private void turnRecognizer_turnCompletedEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            turning = false;
        }

        private void turnRecognizer_turnCanceledEvent(InteractionSourceKind source, Vector3 relativePosition, Ray ray)
        {
            turning = false;
        }
    }
}