
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

        public RecognitionRequest(Image image, string language)
        {
            _image = image;
            _language = language;
        }

        public async Task<string> RecognizeAsync(CancellationToken cancellationToken)
        {
            var imgPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".png");

            using(var file = File.OpenWrite(imgPath))
            {
                await _image.SaveAsPngAsync(file);
                await file.FlushAsync();
                file.Close();
            }

            try
            {
                var results = await ProcessEx.RunAsync("livetext-sharp", $"{imgPath} {_language}");
                return results.ExitCode == 0 ? string.Join("\n", results.StandardOutput) : throw new Exception(string.Join("\n", results.StandardOutput));
            }
            finally
            {
                File.Delete(imgPath);
            }
        }
    }
}

