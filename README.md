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

## Build

1. Build the swift helper as universal binary

```shell
cd swift
xcrun swift build -c release --arch arm64 --arch x86_64
cd ..
```
2. Copy the binary to the csharp directory

```shell
cp ./swift/.build/apple/Products/Release/livetext-sharp ./csharp
```
3. Build the C# package normally

## License

This package is licensed under the MIT License. See the `LICENSE` file for more information.