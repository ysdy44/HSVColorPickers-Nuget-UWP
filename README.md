# HSV Color Pickers

This is a library used to develop the UWP.
ColorPicker, RGBPicker, HSVPicker, WheelPicker, PalettePicker, StrawPicker, HexPicker, SwatchesPicker.

![](https://github.com/ysdy44/HSVColorPickers-Nuget-UWP/blob/master/ScreenShot/ScreenShot001.png)
![](https://github.com/ysdy44/HSVColorPickers-Nuget-UWP/blob/master/ScreenShot/ScreenShot003.png)


## Getting Started

|Key|Value|
|:-|:-|
|System requirements| Windows10 Creators Update or upper|
|Development tool|Visual Studio 2017|
|Programing language|C#|
|Display language|English|

  ![](https://github.com/ysdy44/HSVColorPickers-Nuget-UWP/blob/master/ScreenShot/logo.png)


Search 'HSV Color' in Nuget and download it.


### Example

```xaml


xmlns:pickers="using:HSVColorPickers"


<pickers:ColorPicker/>

<pickers:RGBPicker/>
<pickers:HSVPicker/>
<pickers:WheelPicker/>

<pickers:SwatchesPicker/>
<pickers:HexPicker/>
<pickers:AlphaPicker/>
 

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

PalettePicker.CreateFormHue();
PalettePicker.CreateFormSaturation();
PalettePicker.CreateFormValue();,


```


You can learn more from the demo application:
https://www.microsoft.com/store/productId/9PD2JJZQF524



Enjoy it..
