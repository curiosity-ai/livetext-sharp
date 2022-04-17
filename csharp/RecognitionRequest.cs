
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using RunProcessAsTask;
using System.Threading;
using System.IO;
using System;
using System.Diagnostics;

namespace LiveTextSharp
{
    public sealed class RecognitionRequest
    {
        private readonly Image _image;
        private readonly string _language;

        public static string CliPath { get; set; } = null;

        public RecognitionRequest(Image image, string language)
        {
            _image = image;
            _language = language;
        }

        public async Task<string> RecognizeAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(CliPath))
            {
                CliPath = Path.GetDirectoryName(typeof(RecognitionRequest).Assembly.Location);
            }

            var imgPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");

            using(var file = File.OpenWrite(imgPath))
            {
                await _image.SaveAsPngAsync(file);
                await file.FlushAsync();
                file.Close();
            }

            try
            {
                var psi = new ProcessStartInfo()
                {
                    WorkingDirectory = CliPath,
                    FileName = "livetext-sharp",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                };

                psi.ArgumentList.Add(imgPath);
                psi.ArgumentList.Add(_language);

                var results = await ProcessEx.RunAsync(psi, cancellationToken);

                return results.ExitCode == 0 ? string.Join("\n", results.StandardOutput) : throw new Exception(string.Join("\n", results.StandardOutput));
            }
            finally
            {
                File.Delete(imgPath);
            }
        }
    }
}

