# SkiaSharp Multi-threaded Lists

A small example of how SkiaSharp can be used with multiple threads to draw items in a list.

This example app has nothing special, except for a custom list view cell. This cell handles its data being changed and drawing on a background thread. This keeps the UI alive, while still allowing for some serious drawing code.

The main logic is here: [MultiThreadedLists/SpecialViewCell.xaml.cs](MultiThreadedLists/SpecialViewCell.xaml.cs)
