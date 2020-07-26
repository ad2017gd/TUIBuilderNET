# TUIBuilderNET
C#.NET API for building Text UIs.

# Example
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
