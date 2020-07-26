# TUIBuilderNET
C#.NET API for building Text UIs.
  
🔴 Disclaimer 🔴 Not intended for actual use. This is garbage.  

  
- [Examples](#examples)
- [Documentation](#documentation)
   - [Classes](#classes)
   - [Enums](#enums)
  
  
  
  
# Examples
```csharp
using System;
using TUIBuilderNET;

class Program
{
    static void Main(string[] args)
    {
        var builder = new TUIBuilder();

        var text1   = new TUIBuilder.Text("Hello world!\n\n");
        var button1 = new TUIBuilder.Button("Click to exit.", builder.Quit);

        builder.AddObject(text1);
        builder.AddObject(button1);

        builder.Show();

        Console.WriteLine("User exited TUI.");
    }
}
```
# Documentation
## Classes
  
  
### TUIBuilder 
> TUIBuilder(ConsoleColor select1color = ConsoleColor.DarkBlue, ConsoleColor select2color = ConsoleColor.Blue, object textcolor = null, object bgcolor = null, ConsoleColor placecolor = ConsoleColor.Gray) 

Main class of the API. Creates an instance of the builder.
  
`selection` - ID of currently selected object.  
`colors` - List of colors used for selection, foreground, background etc.  
`map` - List of all added objects.  
`onKeyPress` - Function called on key press. Argument passed is the ConsoleKeyInfo.

`Render()` - Render the current TUI.  
`Show()` - Start main thread (handle keypresses, render) and check thread (render again if terminal size changed).  
`GetSelect(int s)` - Get selectable object from `selectable` list.  
`CheckSelect(TObject obj)` - Check if object is selected.  
`AddObject(TObject obj, int pos = -1)` - Add object to builder.  
`Flush()` - Remove all objects and render.  
`Reset()` - Reset all variables and render.
 

### TUIBuilder.TObject 
> TObject(ObjectType t)  

Base class for all elements.  
  
`id` - ID of object.  
`type` - Type of object ([ObjectType](#enums)).  
`canBeSelected` - Indicates if object is selectable.  
`onSelect` - Function called when object gets selected.  
`Forecolor` - Foreground color of object.
  

### TUIBuilder.Text 
> Text(string text)  

Displays text.   
  
`content` - Contains text to be displayed.
  

### TUIBuilder.FText 
> FText(Func<object\> ret)  

Displays text from a function return value. Useful for dynamic text.  
  
`content` - Indicates the function whose return value shall be used.
  

### TUIBuilder.Button 
> Button(string text, Action func)  

Displays a clickable object, that when clicked, calls a function.  
  
`content` - Contains text to be displayed.  
`onclick` - Function called when button is clicked.
  

### TUIBuilder.Input 
> Input(string phold = "", bool multi = false, int fixedlen = 0)  

When selected, text can be inputted.  
  
`content` - Input value.  
`placeholder` - Centered text that is shown when there is no input.  
`multiline` - Indicates if multi line input is allowed.  
`fixedlength` - If larger than 0, indicates the maximum length of the object, but does not limit input length.
   

### TUIBuilder.Slider 
> Slider(SliderType type = SliderType.BAR, int min = 0, int max = 5, int start = 0)  

When selected, value can be changed using the left-right arrow keys.  
  
`value` - Input value.  
`minvalue`, `maxvalue` - Min value, respectively max value allowed.  
`slidertype` - Type of slider ([SliderType](#enums)).  
`props` - Properties of slider.
  

### TUIBuilder.Picker 
> Picker(IEnumerable<object\> options, PickerType pt = PickerType.LIST)  
    
When selected, selected value can be changed using the left-right arrow keys. 
  
`list` - List of possible values.  
`selection` - Index of selected value.  
`pickertype` - Type of picker ([PickerType](#enums)).
  

### TUIBuilder.Checkmark 
> Checkmark(bool startchecked = false)  

When selected, checkmark can be toggled using ENTER or left-right arrow keys.  
  
`ischecked` - Indicates if checkmark is checked.  
`props` - Properties of checkmark.
  
  
## Enums
  
  
### TUIBuilder.ObjectType
> TEXT  
> FTEXT  
> BUTTON  
> SLIDER  
> INPUT  
> PICKER  
> CHMARK

### TUIBuilder.SliderType
> BLOCK  
> BAR  
> NUMBER

### TUIBuilder.PickerType
> LIST  
> RADIO



