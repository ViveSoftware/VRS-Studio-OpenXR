# VRS Studio
Copyright 2024, HTC Corporation. All rights reserved.

<details>
  <summary>Table of Contents</summary>
  <ol>
    <li><a href="#about">About</a></li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#settings-build-setup">Settings & Build Setup</a></li>
      </ul>
    </li>
    <li><a href="#how-to-play">How to Play</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
    <li><a href="#acknowledgments">Acknowledgments</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About

VRS Studio is a project developed by HTC VIVE to showcase the features on VIVE Focus Vision and other devices, while also demonstrating various use cases of these capabilities using our SDK and VIU toolkit. The Demo primarily includes the following experiences:
    
- Body Tracking
- Faical Tracking
- Spectator Camera
- VIVE Ultimate Tracker
- Realistic Hand Interaction

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

### Built With

* [VIVE OpenXR SDK](https://developer.vive.com/resources/openxr/unity/)
* [VIVE Input Utility for Unity](https://github.com/ViveSoftware/ViveInputUtility-Unity)
* [DOTween](http://dotween.demigiant.com/)

### Prerequisites

- Vive XR Elite (ROM Version 1.0.999.738 above)
- Vive Focus Vision (ROM Version 7.0.999.228 above)
- Unity 2022.3.21f1 (With Android Build Support)
- VIVE OpenXR Plugin 2.5.0 or newer
- VIVE Input Utility 1.20.2 or newer

### Settings & Build Setup

 1. Clone the repo.
  ```sh
    git clone https://github.com/ViveSoftware/VRS-Studio-OpenXR.git
  ```
 2. In Unity Hub, select "Add project from disk" and choose the "VRS_Studio" project folder.
 3. Ensure that the editor version used is "2022.3.21f1" and the platform is set to "Android".
 4. After the project is loaded, verify that the feature is enabled in Project Settings > XR Plug-in Management > OpenXR.
     - Eye Gaze Interaction Profile
     - VIVE Focus 3 Controller Interaction
     - Hand Tracking Subsystem
     - VIVE XR - Interaction Group
        - Vive Hand Interaction
        - Vive XR Tracker
     - VIVE XR Facial Tracking
     - VIVE XR Path Enumeration (Beta)
     - VIVE XR Spectator Camera (Beta)
        - Allow capture panorama
     - VIVE XR Support
5. To build the APK, go to the menu bar and select VRS_Studio > Build > Do Build. The APK will be located in the "builds" folder within the project directory.

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- HOW TO PLAY -->
## How to Play
If you are launching the app for the first time, there is a tutorial will guide you through the all features. If not, you can refer to the gameplay steps below to fully experience all features.

Below is an introduction to each area’s features, starting clockwise from the Computer Area:
1. Computer Area:

    <img src="images/ComputerArea.png" alt="ComputerArea" width="30%">

	- Use your hand or controller to type on the keyboard and input text.
	- Use your hand or controller to grab and move the keyboard.
	- When grabbing the keyboard with both hands or both controllers simultaneously, you can resize the keyboard.

2. Mirror Area:

    <img src="images/LipTracking.gif" alt="LipTracking" width="30%">

	- Enable the facial-tracking and eye-tracking features and bubbles will continuously appear.
	- Staring at a bubble with your eyes will make it grow and eventually burst.
	- Hold your mouth in an "O" shape to blow bubbles from your mouth position.

3. Recording Area:

    <img src="images/SpecCam.gif" alt="SpecCam" width="30%">

	- Turn on the recording feature. Use the left controller to switch between different recording perspectives.
	- Press the Y button on the left controller to take 360-degree photos.

4. Sofa Area:

    <img src="images/SofaArea.png" alt="SofaArea" width="30%">

	- Use your hand or controller to grab a plastic bottle.
	- Throw the bottle into the trash can to hear a notification sound.

5. Exhibition Area:

    <img src="images/ExhibitionArea.png" alt="ExhibitionArea" width="30%">

	- Use your hand or controller to grab any object.
	- When you release the camouflage airplane, 3D spatial sound effects will play around you.

6. Avatar Area:

    <img src="images/BodyTracking.gif" alt="BodyTracking" width="30%">

	- Press the X button on the left controller and the A button on the right controller to enable the calibration feature.
	- Follow the pose of the Avatar until the calibration is complete.
	- Once calibration is complete, the Avatar will follow the movements of your HMD and controllers or hands.

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- LICENSE -->
## License

See `LICENSE.pdf` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- CONTACT -->
## Contact

Project Link: [https://github.com/ViveSoftware/VRS-Studio](https://github.com/ViveSoftware/VRS-Studio-OpenXR.git)

<p align="right">(<a href="#top">back to top</a>)</p>

<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* [My take on shaders: Teleportation dissolve](https://halisavakis.com/my-take-on-shaders-teleportation-dissolve/) - Used as teleportation VFX of the robot

<p align="right">(<a href="#top">back to top</a>)</p>