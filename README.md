# Shared Experience Hololens

Microsoft Hololens Application for shared experience in medical area, developed with Unity. 

## 1. Shared Hologram for multiple Hololens Unit. 
- First unit connected to sharing service acts as host, and other units receives transformation(position, rotation, scale) through service. Codes based on Holograms 240-SharedExperience by Microsoft. 

## 2. Interactions with gestures and keyword recognizer.
  Only host can do the gesture. 
  - Move Model : Start moving the model. Model is placed right in front of host. Can fix the model by air tap.
  - Scale Model : Start scaling the model by vertical navigation.(Hold your finger after air tap, and move vertically)
  - Rotate Model : Start rotating the model by navigation in any direction.
  - Reset Model : Resets the model to original rotation / scale. Model is placed right in front of host.
  - Start Tracking : Start Tracking the model. 
  
## 3. Tracking 
  - Tracking using Vuforia Tracking images. Tracking Image is in VRender/Assets/Vuforia/Target.jpg
  
## 4. Volume Rendering
  - Implemented Volume Rendering of CT scan results into 3D cube. Codes implemented from            http://graphicsrunner.blogspot.kr/2009/02/volume-rendering-201-optimizations.html, 
 https://github.com/gillesferrand/Unity-RayTracing
 
 ![Alt text](chest.png?raw=true "Title")

## 5. Further Application 
  - Model picked from Onedrive / filepicker.
  - Optimization of Volume Rendering, Transfer Function
