import Foundation
import CoreImage
import Cocoa
import Vision

var joiner = " "
var bigSur = false;

if #available(OSX 11, *) {
    bigSur = true;
}

func convertCIImageToCGImage(inputImage: CIImage) -> CGImage? {
    let context = CIContext(options: nil)
    if let cgImage = context.createCGImage(inputImage, from: inputImage.extent) {
        return cgImage
    }
    return nil
}

@available(macOS 10.15, *)
func recognizeTextHandler(request: VNRequest, error: Error?) {
    guard let observations =
            request.results as? [VNRecognizedTextObservation] else {
        return
    }
    let recognizedStrings = observations.compactMap { observation in
        // Return the string of the top VNRecognizedText instance.
        return observation.topCandidates(1).first?.string
    }
    
    // Process the recognized strings.
    let joined = recognizedStrings.joined(separator: joiner)
    print(joined)
    
    let pasteboard = NSPasteboard.general
    pasteboard.declareTypes([.string], owner: nil)
    pasteboard.setString(joined, forType: .string)
    
}

@available(macOS 10.15, *)
func detectText(fileName : URL) -> [CIFeature]? {
    if let ciImage = CIImage(contentsOf: fileName){
        guard let img = convertCIImageToCGImage(inputImage: ciImage) else { return nil}
      
        let requestHandler = VNImageRequestHandler(cgImage: img)

        // Create a new request to recognize text.
        let request = VNRecognizeTextRequest(completionHandler: recognizeTextHandler)
        request.recognitionLanguages = recognitionLanguages
       
        
        do {
            // Perform the text-recognition request.
            try requestHandler.perform([request])
        } catch {
            print("Unable to perform the requests: \(error).")
        }
}
    return nil
}

let arguments: [String] = Array(CommandLine.arguments.dropFirst())

let inputFile: String = arguments[0]

let language: String = arguments[1]

//"/Users/rafael/Downloads/243903.png" en-US

let inputURL = URL(fileURLWithPath: inputFile)
var recognitionLanguages = [language]

do {
    if let features = detectText(fileName : inputURL), !features.isEmpty{}

} catch {
    print("Whoops! An error occurred: \(error)")
}

exit(EXIT_SUCCESS)
