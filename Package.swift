// swift-tools-version:5.5
// The swift-tools-version declares the minimum version of Swift required to build this package.

import PackageDescription

let package = Package(
    name: "livetext-sharp",
    platforms: [
        .macOS(.v10_15)
    ],
    dependencies: [
    ],
    targets: [
        .executableTarget(
            name: "livetext-sharp",
            dependencies: [])
    ]
    
)
