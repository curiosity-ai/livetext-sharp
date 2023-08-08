using System.Threading.Tasks;
using SixLabors.ImageSharp;
using RunProcessAsTask;
using System.Threading;
using System.IO;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace LiveTextSharp
{
    public sealed class RecognitionRequest
    {
        private readonly Image  _image;
        private readonly string _language;

        public static string WorkingDirectory { get; set; } = null;
        public static string CliPath          { get; set; } = null;
        private const string CliExeName = "livetext-sharp";

        public static bool IsSupported     => OperatingSystem.IsMacOSVersionAtLeast(10, 15);
        public static bool CanSetLanguages => OperatingSystem.IsMacOSVersionAtLeast(11);

        public RecognitionRequest(Image image, params string[] languages)
        {
            _image = image;

            if (OperatingSystem.IsMacOSVersionAtLeast(11))
            {
                _language = string.Join(";", languages);
            }
            else
            {
                _language = "en-US";
            }
        }

        private void TryFindCLI()
        {
            if (string.IsNullOrWhiteSpace(CliPath))
            {
                var assmeblyDir = typeof(RecognitionRequest).Assembly.Location;

                WorkingDirectory = Path.GetDirectoryName(assmeblyDir);
                CliPath          = Path.Combine(WorkingDirectory, CliExeName);

                if (File.Exists(CliPath))
                {
                    return;
                }
                else
                {
                    if (RuntimeInformation.RuntimeIdentifier.Contains("osx"))
                    {
                        switch (RuntimeInformation.ProcessArchitecture)
                        {
                            case Architecture.Arm:
                            case Architecture.Arm64:
                            {
                                CliPath = Path.Combine(WorkingDirectory, "runtimes", "osx-arm64", "native", CliExeName);

                                if (!File.Exists(Path.Combine(CliPath)))
                                {
                                    throw new InvalidOperationException($"Could not find {CliPath}");
                                }
                                return;
                            }
                            default:
                            {
                                CliPath = Path.Combine(WorkingDirectory, "runtimes", "osx-x64", "native", CliExeName);

                                if (!File.Exists(CliPath))
                                {
                                    throw new InvalidOperationException($"Could not find {CliPath}");
                                }
                                return;
                            }
                        }
                    }
                }
                throw new InvalidOperationException($"Could not find {CliPath}");
            }
        }


        public async Task<LiveTextBlock[]> RecognizeAsync(CancellationToken cancellationToken)
        {
            TryFindCLI();

            var imgPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");

            using (var file = File.OpenWrite(imgPath))
            {
                await _image.SaveAsPngAsync(file, cancellationToken);
                await file.FlushAsync(cancellationToken);
                file.Close();
            }

            try
            {
                var psi = new ProcessStartInfo()
                {
                    WorkingDirectory = WorkingDirectory,
                    FileName         = CliPath,
                    CreateNoWindow   = true,
                    WindowStyle      = ProcessWindowStyle.Hidden
                };

                psi.ArgumentList.Add(imgPath);
                psi.ArgumentList.Add(_language);

                var results = await ProcessEx.RunAsync(psi, cancellationToken);

                return results.ExitCode == 0 ? JsonSerializer.Deserialize<LiveTextBlock[]>(string.Join('\n', results.StandardOutput)) : throw new Exception(string.Join("\n", results.StandardOutput));
            }
            finally
            {
                File.Delete(imgPath);
            }
        }
    }

    public sealed class LiveTextBlock
    {
        [JsonPropertyName("bounds")] public LiveTextBounds Bounds { get; set; }
        [JsonPropertyName("text")]   public string         Text   { get; set; }
    }

    public sealed class LiveTextBounds
    {
        [JsonPropertyName("bottomRight")] public double[] BottomRight { get; set; }
        [JsonPropertyName("topRight")]    public double[] TopRight    { get; set; }
        [JsonPropertyName("topLeft")]     public double[] TopLeft     { get; set; }
        [JsonPropertyName("bottomLeft")]  public double[] BottomLeft  { get; set; }
    }
}