import Foundation
import CoreImage
import Cocoa
import Vision

var joiner = " "

func convertCIImageToCGImage(inputImage: CIImage) -> CGImage? {
    let context = CIContext(options: nil)
    if let cgImage = context.createCGImage(inputImage, from: inputImage.extent) {
        return cgImage
    }
    return nil
}

@available(macOS 10.15, *)
func recognizeTextHandler(request: VNRequest, error: Error?) {
    guard let observations = request.results as? [VNRecognizedTextObservation] else { return }
    let recognizedStrings = observations.compactMap { observation in return observation.topCandidates(1).first?.string }
    let joined = recognizedStrings.joined(separator: joiner)
    print(joined)
}

@available(macOS 10.15, *)
func detectText(fileName : URL, languages : [String]) {
    if let ciImage = CIImage(contentsOf: fileName) 
    {
        guard let img = convertCIImageToCGImage(inputImage: ciImage) else { return }
      
        let requestHandler = VNImageRequestHandler(cgImage: img)
        
        let request = VNRecognizeTextRequest(completionHandler: recognizeTextHandler)

        request.recognitionLanguages = languages
        
        do 
        {
            try requestHandler.perform([request])
        }
        catch 
        {
            print("Error: \(error)")
            exit(0xDEAD)
        }
}
    return
}

let arguments: [String] = Array(CommandLine.arguments.dropFirst())
let inputFile: String = arguments[0].trimmingCharacters(in: CharacterSet(charactersIn: "\""))
let language: String = arguments[1]

let inputURL = URL(fileURLWithPath: inputFile)
var recognitionLanguages = language.components(separatedBy: ";")

detectText(fileName : inputURL, languages: recognitionLanguages)

exit(EXIT_SUCCESS)
