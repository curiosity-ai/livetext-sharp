# LiveText C# Wrapper

This package provides a C# wrapper for the macOS LiveText API, allowing developers to easily integrate LiveText functionality into their C# applications.

## Installation

To install the LiveText API C# wrapper, simply add the [NuGet Package](https://www.nuget.org/packages/LiveTextSharp) to your project.

## Usage

To use the LiveText API C# wrapper, simply create a new instance of the `RecognitionRequest` class and call the RecognizeAsync method to get the output.

```csharp
var image = ...;
var request = new RecognitionRequest(image);
string recognizedText = request.RecognizeAsync();
```

## License

This package is licensed under the MIT License. See the `LICENSE` file for more information.