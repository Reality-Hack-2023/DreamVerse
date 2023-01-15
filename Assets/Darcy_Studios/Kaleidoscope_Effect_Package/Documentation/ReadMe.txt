Kaleidoscope Effect Documentation

Contents
1. Description
2. Example Scene
3. Scripts
4. Prefabs
5. Sprites and Materials
6. Customisation
7. Contact

1. Description
This effect works by editing the projection matrices of 4 cameras, (found within the Prefabs folder) in order 
to flip their viewports on either the x, y or x and y axis.

2. Example Scene
There is an example scene contained within the Example_Scene folder. This scene includes the kaleidoscope 
camera prefab and an example VFX game object. 

3. Scripts
The C# script responsible for controlling the cameras is called Kaleidoscope.cs and can be found within the 
Scripts folder. 
To use this script on your own custom camera, you simply need to attach it to the camera in the inspector 
and set the axis to flip the camera on using the checkboxes provided. 
Be sure to remove any extra Audio Listeners from your cameras, (if your scene contains more that one) in the 
inspector for each camera, as this can cause an error. 

4. Prefabs
The prefabs folder contains a ready to use camera prefab which is made up of 4 cameras, set to different 
screen positions, which can be edited as desired, a global volume for controlling post-processing effects, 
(post-processing package is required for this feature to operate correctly), 3 varying Kaleidoscope particle 
system examples and a single game object containing all 3, which can be added to any scene. Each of these 
effects can be edited as desired. You may also wish to create your own particle systems for use with the 
kaleidoscope effect cameras.

5. Sprites and Materials
There is a custom sprite within the Sprites folder that is used in a custom material within the Materials 
folder. This material is used as the trail material for the particle system prefabs. To use this material drag 
and drop it into the Trail Renderer slot in the particle system component, under the Renderer tab. 

6. Customisation
It is possible to customise any aspect of the kaleidoscope effect. To change the axis the cameras are flipped
on, simply click either checkbox in the Kaleidoscope script, attached to the cameras. This can be done either 
in Edit or Play Mode.

Feel free to add your own custom sprites to replace the default particle sprite and material, in order to create
a more interesting particle effect. You can also create custom trail sprites and materials. When creating a custom 
particle sprite please ensure that the image is square. This will avoid unwanted stretching. Set the Sprite's 
Texture Type to Sprite (2D and UI). For trail sprites ensure that the trail starts on the far left-hand side of
the image. This will ensure that the trail starts exactly on the particle and not some distance behind it. 

7. Contact
Please feel free to contact me if you have any questions or suggestions for how I could improve this package. 
I am always happy to help out if I can, and appreciate feedback on ways I could improve in the future. 


Thank you for purchasing this package and I hope you have fun making some cool looking kaleidoscope effects!

Joe Darcy
Darcy Studios
 
