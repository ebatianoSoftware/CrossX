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

✔️Label
✔️Button
✔️IconButton
✔️ImageView 
✔️Slider
✔️ProgressBar
✔️ToggleButton 
✔️CheckBox
✔️RadioButton
✔️ItemsView
✔️FrameLayout
✔️StackLayout
✔️WrapLayout
✔️SplitLayout
✔️ToolTips

🔻ContextMenu
🔻ScrollBar
🔻ScrollView
🔻TreeView
🔻ListBox
🔻TextBox
🔻Text
🔻HtmlView

## MVVM Pattern
The framework is strongly based on Model-View-ViewModel pattern. No view code by default (can be realised though with specialized control subclassing) and heavy use of bindings force you to think in Model->ViewModel->View direction in every aspect.

## Skia based rendering
Rendering currently is realised with [SkiaSharp](https://github.com/mono/SkiaSharp) which allows you to create beautiful applications.

## Other backends planned
I plan adding more backends - DirectX, OpenGL, Metal for fast rendering (skipping Skia backend totally) with possibility to access those backends native APIs.

## XX Definition Files
XX definition files are XML files used to describe views and define styles and resources. I chose a XAML-like approach but simpler in its form. The XX "language" will surely evolve into something more in the future. 

Currently XX project is put into CrossX repository and grows with the framework. 

To improve developer experience, a small tool - XxSchemaGenerator (xxsgen) was created. It generates XSD schema files based on your assemblies that use XX contracts. Thanks to that, editing views is easier with attribute names and values hints and values syntax checking.

<br/>
<p align="center"><i>Example definition of CrossX's View</i></p>

```xml
<FrameLayout
  xmlns="https://crossx.support/Schemas/CrossX.Framework.UI"
  xmlns:ex="https://crossx.support/Schemas/CrossX.Example"
  BackgroundColor="Black">
  
  <Label Classes="Header Light" Text="Hello World"/>
  
  <Label Id="label1" Text="&#xe876;" 
         FontFamily="Material Icons" FontSize="30" FontWeight="Bold" 
         TextColor="White" Margin="10"
         HorizontalAlignment="Center" VerticalAlignment="Center"/>
  
  <Button Width="200" Text="Test Button" Margin="0,0,0,50"
          HorizontalAlignment="Center" VerticalAlignment="End"
          BackgroundColor="{Theme SystemBackgroundColor}"
          Command="{Binding TestCommand}" CommandParameter="{Binding}"/>
  
</FrameLayout>
```

## Roadmap

#### [Mobile] AppShell
for better mobile apps experience - shell with flyout menu

#### [Mobile] Android support
#### [Mobile] Touch input
#### [Mobile] iOS support

#### [Desktop] MacOS support

#### [Games][Desktop] GamePad controls navigation system
#### [Games][Desktop] Universal Windows support with DirectX
#### [Games][Desktop] Windows support width DirectX
#### [Games][Desktop] MacOS support with Metal

#### [Games][Mobile] Android support with OpenGL ES    

## Support and Contributions
If you think you have found a bug or have a feature request, use our [issue tracker](https://github.com/ebatianoSoftware/CrossX/issues). Before opening a new issue, please search to see if your problem has already been reported. Try to be as detailed as possible in your issue reports.

If you are interested in contributing fixes or features to CrossX, just fork this repository, make your changes and open [pull request](https://github.com/ebatianoSoftware/CrossX/pulls) to merge your work into development branch. 

⚠️ *Be aware that not all changes will be approved and merged.*

## License
The CrossX project is under the MIT License. See the LICENSE.txt file for more details. Third-party libraries used by CrossX are under their own licenses. Please refer to those libraries for details on the license they use.
