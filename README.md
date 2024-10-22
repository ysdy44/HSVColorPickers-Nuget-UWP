# HSV Color Pickers

This is a library used to develop the UWP.
ColorPicker, RGBPicker, HSVPicker, WheelPicker, PalettePicker, StrawPicker, HexPicker, SwatchesPicker.

![](ScreenShot/ScreenShot001.png)
![](ScreenShot/ScreenShot003.png)


## Getting Started

|Key|Value|
|:-|:-|
|System requirements| Windows10 Creators Update or upper|
|Development tool|Visual Studio 2017|
|Programing language|C#|
|Display language|English|

  ![](ScreenShot/logo.png)


Search 'HSV Color' in Nuget and download it.  
  ![](ScreenShot/Thumbnails000.jpg)


### Example

Run the "TestApp".

```xaml
xmlns:hSVColorPickers="using:HSVColorPickers"


<hSVColorPickers:ColorPicker/>

<hSVColorPickers:RGBPicker/>
<hSVColorPickers:HSVPicker/>
<hSVColorPickers:WheelPicker/>

<hSVColorPickers:SwatchesPicker/>
<hSVColorPickers:HexPicker/>
<hSVColorPickers:AlphaPicker/>

<hSVColorPickers:PaletteHuePicker/>
<hSVColorPickers:PaletteSaturationPicker/>
<hSVColorPickers:PaletteValuePicker/>
 
<hSVColorPickers:CirclePicker/>
```
or 

```csharp
using HSVColorPickers;


new ColorPicker();

new RGBPicker();
new HSVPicker();
new WheelPicker();

new SwatchesPicker();
new HexPicker();
new AlphaPicker();

new PaletteHuePicker();
new PaletteSaturationPicker();
new PaletteValuePicker();
 
new CirclePicker(); 
```


## Learn More

You can learn more from the demo application in windows 10 store:<br/>
[FanKit](https://www.microsoft.com/store/productId/9PD2JJZQF524)


1.Click on item "Colors" in the top bar.  
  ![](ScreenShot/Thumbnails001.jpg)


2.Look for simple examples.  
    ![](ScreenShot/Thumbnails002.jpg)


Enjoy it..
