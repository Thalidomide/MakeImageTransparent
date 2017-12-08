using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ImageUtils
{
    public class RunGenerateImages
    {
        public void Run(string sourcePath, string sourceFileOrNullIfAll)
        {
            if (string.IsNullOrEmpty(sourceFileOrNullIfAll) || sourceFileOrNullIfAll.ToLowerInvariant() == "all")
            {
                TransformAllImagesInPath(sourcePath);
            }
            else
            {
                TransformAndSaveImageWithAllStrategies(sourcePath, new FileNameAndExtension(sourceFileOrNullIfAll));
            }
        }

        private static void TransformAllImagesInPath(string sourcePath)
        {
            var dirInfo = Directory.EnumerateFiles(sourcePath);

            foreach (var info in dirInfo)
            {
                var fileInfo = new FileNameAndExtension(info);

                if (fileInfo.IsSupportedImage())
                {
                    TransformAndSaveImageWithAllStrategies(sourcePath, fileInfo);
                }
            }
        }

        private static void TransformAndSaveImageWithAllStrategies(string path, FileNameAndExtension file)
        {
            Console.WriteLine($"Transform image {file.FileNameWithExtension}");

            using (var image = Image.FromFile(Path.Combine(path, file.FileNameWithExtension)))
            {
                TransformAndSave(image, MainColorStrategy.MostUsed, path, file.FileName);
                TransformAndSave(image, MainColorStrategy.MostDifferentFromMostUsed, path, file.FileName);
                TransformAndSave(image, MainColorStrategy.SecondMostUsed, path, file.FileName);
                TransformAndSave(image, MainColorStrategy.DifferentAndWellUsed, path, file.FileName);
            }
        }

        private static void TransformAndSave(Image image, MainColorStrategy colorStrategy, string sourcePath, string fileName)
        {
            var makeImageTransparent = new ImageTransparentOneColor();

            var savePath = Path.Combine(sourcePath, "FixedTransparency");
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            using (var imageFixed = makeImageTransparent.DetectMainColor(image, colorStrategy))
            {
                var saveFilePath = Path.Combine(savePath, $"{fileName}-trans-strategy-{colorStrategy}.png");

                Console.WriteLine($"Save image to {saveFilePath}");

                imageFixed.Save(saveFilePath);
            }
        }
    }

    public class FileNameAndExtension
    {
        public string FileNameWithExtension { get; }
        public string FileName { get; }
        public string Extension { get; }

        private static readonly IList<string> SupportedImageFormats = new List<string>{".jpg", ".jpeg", ".png"};

        public FileNameAndExtension(string fileNameWithExtension)
        {
            //var extensionIndex = fileNameWithExtension.LastIndexOf(".", StringComparison.Ordinal);

            //FileNameWithExtension = fileNameWithExtension;
            //FileName = fileNameWithExtension.Substring(0, extensionIndex);
            //Extension = fileNameWithExtension.Substring(extensionIndex);
            FileNameWithExtension = Path.GetFileName(fileNameWithExtension);
            FileName = Path.GetFileNameWithoutExtension(fileNameWithExtension);
            Extension = Path.GetExtension(FileNameWithExtension);
        }

        public bool IsSupportedImage()
        {
            return SupportedImageFormats.Contains(Extension.ToLowerInvariant());
        }
    }
}
