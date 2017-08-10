# Shared Experience Hololens

Microsoft Hololens Application for shared experience in medical area, developed with Unity. Github can't carry the model, so only the essential scripts are updated for version controll for now. In the future, whole application will be uploaded.

## 1. Shared Hologram for multiple Hololens Unit. 
- First unit connected to sharing service acts as host, and other units receives transformation(position, rotation, scale) through service. Codes based on Holograms 240-SharedExperience by Microsoft. 

## 2. Interactions with gestures and keyword recognizer.
  Only host can do the gesture. 
  - Move Model : Start moving the model. Model is placed right in front of host. Can fix the model by air tap.
  - Scale Model : Start scaling the model by vertical navigation.(Hold your finger after air tap, and move vertically)
  - Rotate Model : Start rotating the model by navigation in any direction.
  - Reset Model : Resets the model to original rotation / scale. Model is placed right in front of host.
  
## 3. Further Application
  - Adjust the remote hololens unit position by keyword recognizer.
  - Model picked from Onedrive / filepicker.
  - User can enter IP address to look for sharing service.
  
  - Real World Anchor
  - Analyzing volumetric data
