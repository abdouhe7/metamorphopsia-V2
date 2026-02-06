# metamorphopsia-VRproject
This project is created based on Unity 2021.3 and HTC Vive Pro. Before run this program, please enable the external camera at SteamVR settings and do test if the camera could work. 

For the handle key map: Menu button to call UI;
                        Trigger to select UI elements and subdivide grid on test scene.
                        Left pad tracker to move the selected vertex (Keep Pressed).
                        right pad tracker to select vertex (Press).

[UI items]</br>

![image](https://user-images.githubusercontent.com/50432013/186716078-9151d6b8-94a4-424b-90c9-b92fd46095e8.png)</br>
[GO Test Scene] Change to the eye test scene.</br>
[Quit] Quit the application.</br>
[Fish eyes] Change the display mode video stream accoding to fish eyes mode.</br>
[Monocular] The left eye and right eye use differnt distortion template (the templates were made in eye test scene)[Recommend].</br>
[Binocular] The both eyes use same distortion template (the template was made in eye test scene).</br></br>


![image](https://user-images.githubusercontent.com/50432013/186719341-146b512f-8022-4d1e-96d4-cbc50704675b.png)</br>
[Save] Save current template to the selected eye item.</br>
[Read] Read the template data from json file of the selected eye item .</br>
[Recover Sample] Recover the template to initial state.</br>
[Both eyes] Each eye use the same template, corresponding to the binocular mode, would distort the video by same pattern.</br>
[Left eye] Left eye's template, corresponding to the monocular mode, would only distort left external camera's video content.</br>
[Right eye] Right eye's template, corresponding to the monocular mode, would only distort right external camera's video content.</br>
</br>
Press the left pad tracker's right axis to move to another UI panel.</br>
![image](https://user-images.githubusercontent.com/50432013/186721979-ee704278-f924-415a-8484-5cbb20262d06.png)</br>
[Play] Change to the display scene (ps:would not save automatically, please save before leave).</br>
[Quit] Quit the application.</br>
[Show Grid] Would change the adjustment mode to only selected point and line show.</br>
[Show Texture] Show a texture to assist adjustment.</br> 
![image](https://user-images.githubusercontent.com/50432013/186755750-3c12315d-f6bc-4fba-a870-e20c52ee008a.png)</br></br>

```diff
- The execution program stay in "Exe" file in red.
```
**PS: Don't change the execution file name, it may causes the input system failed.**
