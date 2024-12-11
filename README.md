<div style="text-align: center;"> 
<h1>ImageManipulator</h1>
</div>

![](https://i.imgur.com/sFxOWcm.png)

<div style="text-align: center;"> 
<h2>Description</h2>
</div>

ImageManipulator is a piece of software to utilize partial functionality of popular [ImageJ](https://imagej.net/ij/), which is used to scientific image analysis and processing.
Reason to write ImageManipulator was to get familiar with popular image processing algorithm and getting known C# unsafe code with pointers to process large pictures in finite amount of time.

### Getting started

**Only windows platform supported!**
1. Clone repository
2. Navigate to cloned directory
3. Execute `dotnet build --configuration Release --output ./Release`
4. Launch `ImageManipulator.Presentation.exe` in `Release` directory

### Features
- Image managing
  - Opening an image
  - Image changes history
  - Saving image
  - Edit multiple images
- Image processing
  - Duplication
  - Negation
  - [Contrast stretching](https://homepages.inf.ed.ac.uk/rbf/HIPR2/stretch.htm)
  - [Non-Linear contrast stretching](https://learnopengl.com/Advanced-Lighting/Gamma-Correction)
  - [Histogram equalization](https://towardsdatascience.com/histogram-equalization-5d1013626e64)
  - Arithmetic operations on image (Add, Subtract, Multiply, Divide, Amplitude, etc.)
  - Bitwise operations on image (AND, OR, NOT, Shift, etc.)
  - [Image convolution](https://vincmazet.github.io/bip/filtering/convolution.html)
    - [Moving average](https://en.wikiversity.org/wiki/Moving_Average/Image_Processing)
    - [Gaussian smoothing](https://homepages.inf.ed.ac.uk/rbf/HIPR2/gsmooth.htm)
    - [Laplacian filters](https://www.sciencedirect.com/topics/engineering/laplacian-filter)
    - [Sobel edge detection](https://homepages.inf.ed.ac.uk/rbf/HIPR2/sobel.htm)
    - [Laplacian of gaussian](https://homepages.inf.ed.ac.uk/rbf/HIPR2/log.htm)
    - [Prewitt edge detection](https://en.wikipedia.org/wiki/Prewitt_operator)
    - [Canny edge detection](http://midag.cs.unc.edu/pubs/CScourses/254-Spring2002/Canny1986.pdf)

### What you can do with ImageManipulator?
Examples:
- Apply threshold
![](https://i.imgur.com/DPVeGXX.png)
- Canny edge detection
![](https://i.imgur.com/ir2Bu2u.png)
- See changes, histogram and revert to previous change
![](https://i.imgur.com/UUQPd6D.png)
- Apply bitwise operations
![](https://i.imgur.com/Y7qCE8N.png)
- Apply arithmetic operations
![](https://i.imgur.com/ZTjy8UN.png)
- Negate image
![](https://i.imgur.com/Yqsp0E7.png)

### Technologies
ImageManipulator consist of few technologies:
- .NET8
- WPF - as a base for development window application
- AvaloniaUI - user control and look
- ReactiveUI - provide building blocks and reusable implementation for user controls
- CommunityToolkitMVVM - providing standard, self-containing types for implementation using MVVM pattern 
- Microsoft DependencyInjection - provide IOC container for dependent implementations 
- LiveCharts - histogram graphs
- Scrutor - scan for CQRS implementation
- Moq - testing purposes
- NUnit 3 - testing purposes