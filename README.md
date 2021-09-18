<p align="center">
  <img src="CrossX.png">
</p>

# CrossX

CrossX is an open source cross-platform framework for building iOS, Android, Windows, UWP and MacOS apps and 2D games with .NET from a single codebase.

Fast & easy app development with focus on performance and ablility to run in two redraw modes:
* **continous** - for games,
* **smart redraw** - for apps (content is redrawn only when a change is signaled.

CrossX comes with many built in layouts and controls to build and design apps quickly from a single API. You can subclass any control to customize their behavior or define your own controls.

⚠️*Framework is in early stage of development and cannot be used yet. Please come back later to check its app development readiness status.*

## MVVM Pattern
The framework is strongly based on Model-View-ViewModel pattern. No view code by default (can be realised though with specialized control subclassing) and heavy use of bindings force you to think in Model->ViewModel->View direction in every aspect.

## Skia based rendering
Rendering currently is realised with [SkiaSharp](https://github.com/mono/SkiaSharp) which allows you to create beautiful applications.

## Support and Contributions
If you think you have found a bug or have a feature request, use our issue tracker. Before opening a new issue, please search to see if your problem has already been reported. Try to be as detailed as possible in your issue reports.

If you are interested in contributing fixes or features to CrossX, just fork this repository, make your changes and open Pull Request to merge your work into development branch (Be aware that not all changes will be approved and merged).

## License
The CrossX project is under the MIT License. See the LICENSE.txt file for more details. Third-party libraries used by CrossX are under their own licenses. Please refer to those libraries for details on the license they use.
