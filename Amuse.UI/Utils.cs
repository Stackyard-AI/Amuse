﻿using Amuse.UI.Models;
using Models;
using OnnxStack.StableDiffusion.Config;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Amuse.UI
{
    public static class Utils
    {
        public static string GetAppVersion()
        {
            var version = Assembly.GetEntryAssembly().GetName().Version;
            return $"v{version.Major}.{version.Minor}.{version.Build}";
        }

        public static string GetImageCacheDirectory(string subDirectory = null, bool createSubDirectory = false)
        {
            var cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".cache");
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            if (string.IsNullOrEmpty(subDirectory))
                return cachePath;

            cachePath = Path.Combine(cachePath, subDirectory);
            if (createSubDirectory && !Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            return cachePath;
        }

        public static string RandomString()
        {
            return Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
        }

        public static BitmapImage CreateBitmap(byte[] imageBytes)
        {
            using (var memoryStream = new MemoryStream(imageBytes))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memoryStream;
                image.EndInit();
                return image;
            }
        }

        public static byte[] GetImageBytes(this BitmapSource image)
        {
            if (image == null)
                return null;

            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }

        public static async Task<bool> SaveImageFileAsync(this BitmapSource image, string filename)
        {
            await Task.Run(() =>
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                using (var fileStream = new FileStream(filename, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
            });
            return File.Exists(filename);
        }


        public static async Task<bool> SaveBlueprintFileAsync(this ImageResult imageResult, string filename)
        {
            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter() }
            };
            using (var fileStream = new FileStream(filename, FileMode.Create))
            {
                await JsonSerializer.SerializeAsync(fileStream, imageResult, serializerOptions);
                return File.Exists(filename);
            }
        }


        public static async Task<bool> AutoSaveAsync(this ImageResult imageResult, string autosaveDirectory, bool includeBlueprint)
        {
            if (!Directory.Exists(autosaveDirectory))
                Directory.CreateDirectory(autosaveDirectory);

            var random = RandomString();
            var imageFile = Path.Combine(autosaveDirectory, $"image-{imageResult.SchedulerOptions.Seed}-{random}.png");
            var blueprintFile = Path.Combine(autosaveDirectory, $"image-{imageResult.SchedulerOptions.Seed}-{random}.json");
            if (!await imageResult.Image.SaveImageFileAsync(imageFile))
                return false;

            if (includeBlueprint)
                return await imageResult.SaveBlueprintFileAsync(blueprintFile);

            return true;
        }


        public static SchedulerOptions ToSchedulerOptions(this SchedulerOptionsModel model)
        {
            return new SchedulerOptions
            {
                AlphaTransformType = model.AlphaTransformType,
                BetaEnd = model.BetaEnd,
                BetaStart = model.BetaStart,
                BetaSchedule = model.BetaSchedule,
                ClipSample = model.ClipSample,
                ClipSampleRange = model.ClipSampleRange,
                GuidanceScale = model.GuidanceScale,
                Height = model.Height,
                InferenceSteps = model.InferenceSteps,
                MaximumBeta = model.MaximumBeta,
                PredictionType = model.PredictionType,
                SampleMaxValue = model.SampleMaxValue,
                Seed = model.Seed,
                StepsOffset = model.StepsOffset,
                Width = model.Width,
                Strength = model.Strength,
                Thresholding = model.Thresholding,
                TimestepSpacing = model.TimestepSpacing,
                TrainedBetas = model.TrainedBetas,
                TrainTimesteps = model.TrainTimesteps,
                UseKarrasSigmas = model.UseKarrasSigmas,
                VarianceType = model.VarianceType,
                OriginalInferenceSteps = model.OriginalInferenceSteps,
                SchedulerType = model.SchedulerType
            };
        }

        public static SchedulerOptionsModel ToSchedulerOptionsModel(this SchedulerOptions model)
        {
            return new SchedulerOptionsModel
            {
                AlphaTransformType = model.AlphaTransformType,
                BetaEnd = model.BetaEnd,
                BetaStart = model.BetaStart,
                BetaSchedule = model.BetaSchedule,
                ClipSample = model.ClipSample,
                ClipSampleRange = model.ClipSampleRange,
                GuidanceScale = model.GuidanceScale,
                Height = model.Height,
                InferenceSteps = model.InferenceSteps,
                MaximumBeta = model.MaximumBeta,
                PredictionType = model.PredictionType,
                SampleMaxValue = model.SampleMaxValue,
                Seed = model.Seed,
                StepsOffset = model.StepsOffset,
                Width = model.Width,
                Strength = model.Strength,
                Thresholding = model.Thresholding,
                TimestepSpacing = model.TimestepSpacing,
                TrainedBetas = model.TrainedBetas,
                TrainTimesteps = model.TrainTimesteps,
                UseKarrasSigmas = model.UseKarrasSigmas,
                VarianceType = model.VarianceType,
                OriginalInferenceSteps = model.OriginalInferenceSteps,
                SchedulerType = model.SchedulerType
            };
        }

        public static PromptOptionsModel ToPromptOptionsModel(this PromptOptions promptOptions)
        {
            return new PromptOptionsModel
            {
                Prompt = promptOptions.Prompt,
                NegativePrompt = promptOptions.NegativePrompt
            };
        }



        public static BatchOptionsModel ToBatchOptionsModel(this BatchOptions batchOptions)
        {
            return new BatchOptionsModel
            {
                BatchType = batchOptions.BatchType,
                ValueTo = batchOptions.ValueTo,
                Increment = batchOptions.Increment,
                ValueFrom = batchOptions.ValueFrom
            };
        }


        public static BatchOptions ToBatchOptions(this BatchOptionsModel batchOptionsModel)
        {
            return new BatchOptions
            {
                BatchType = batchOptionsModel.BatchType,
                ValueTo = batchOptionsModel.ValueTo,
                Increment = batchOptionsModel.Increment,
                ValueFrom = batchOptionsModel.ValueFrom
            };
        }

        internal static async Task RefreshDelay(long startTime, int refreshRate, CancellationToken cancellationToken)
        {
            var endTime = Stopwatch.GetTimestamp();
            var elapsedMilliseconds = (endTime - startTime) * 1000.0 / Stopwatch.Frequency;
            int adjustedDelay = Math.Max(0, refreshRate - (int)elapsedMilliseconds);
            await Task.Delay(adjustedDelay, cancellationToken).ConfigureAwait(false);
        }

        public static void LogToWindow(string message)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                (System.Windows.Application.Current.MainWindow as MainWindow).UpdateOutputLog(message);
            }));
        }


        /// <summary>
        /// Forces the notify collection changed event.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">The collection.</param>
        public static void ForceNotifyCollectionChanged<T>(this ObservableCollection<T> collection)
        {
            // Hack: Moving an item will invoke a collection changed event
            collection?.Move(0, 0);
        }
    }
}
