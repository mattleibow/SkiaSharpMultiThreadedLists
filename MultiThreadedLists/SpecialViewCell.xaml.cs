using System;
using System.Threading;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace MultiThreadedLists
{
    public partial class SpecialViewCell : ViewCell
    {
        // these items change per data item
        private CancellationTokenSource cancellation;
        private SKImage image;

        // the current render task so we don't queue up multiple
        private Task currentTask;

        // a lock to prevent a dispose and a draw at the same time
        private object imageLocker = new object();

        public SpecialViewCell()
        {
            InitializeComponent();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // we have to release any old data
            ResetImageAndTask();

            // let the canvas know that we have new data
            canvasview.InvalidateSurface();
        }

        // this method does no render work at all, just copies the image to the screen
        // if there is no image yet, then it asks for one to be created
        // if an image is already in the works, then just render nothing
        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;

            // prepare the surface
            canvas.Clear(SKColors.Transparent);

            // if there is an image, then draw it
            var hasImage = false;
            lock (imageLocker)
            {
                if (image != null)
                {
                    hasImage = true;

                    canvas.DrawImage(image, 0, 0);
                }
            }

            // if there was no image, ask for one to be created
            if (!hasImage && currentTask == null)
            {
                // fire and forget the generator
                currentTask = StartRecreateImageTask(e.Info.Size, BindingContext as DataItem);
            }
        }

        // this method does the work to manage the image creatiion and disposal
        // must always be called from the main thread
        private async Task StartRecreateImageTask(SKSizeI size, DataItem data)
        {
            ResetImageAndTask();

            // prepare to start a background task
            cancellation = new CancellationTokenSource();

            // we need to track this token for this particular item
            var token = cancellation.Token;

            // start the new task
            var newImage = await Task.Run(() => GenerateImage(size, data, token));

            // we are finished, so update the cell only if we still want the
            // image that started this whole operation
            if (!token.IsCancellationRequested)
            {
                lock (imageLocker)
                {
                    image = newImage;
                }

                // redraw cell with the new image
                canvasview.InvalidateSurface();
            }

            // we are done, so reset the tasks
            currentTask = null;
        }

        // must always be called from the main thread
        private void ResetImageAndTask()
        {
            // stop any old tasks
            cancellation?.Cancel();

            // clear any old data
            SKImage tmp;
            lock (imageLocker)
            {
                tmp = image;
                image = null;
                currentTask = null;
            }
            tmp?.Dispose();

            // we have no more tasks
            currentTask = null;
        }

        // this method just creates the image
        // it is good to keep it static as it cannot depend on any value of the
        // cell, because, the cell can change at any time - even mid-draw
        private static SKImage GenerateImage(SKSizeI size, DataItem data, CancellationToken cancellationToken)
        {
            // the view is not yet ready
            if (size.Width <= 0 || size.Height <= 0)
                return null;

            var info = new SKImageInfo(size.Width, size.Height);

            using (var surface = SKSurface.Create(info))
            {
                var canvas = surface.Canvas;

                // we may have been asked to cancel, so jump out
                if (cancellationToken.IsCancellationRequested)
                    return null;

                // HACK: some complex drawing
                canvas.DrawComplexThing();

                // we may have been asked to cancel, so jump out
                if (cancellationToken.IsCancellationRequested)
                    return null;

                canvas.Clear(SKColors.Transparent);

                // we may have been asked to cancel, so jump out
                if (cancellationToken.IsCancellationRequested)
                    return null;

                // HACK: some complex drawing
                canvas.DrawComplexThing();

                // we may have been asked to cancel, so jump out
                if (cancellationToken.IsCancellationRequested)
                    return null;

                // draw the background
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Color = data.Color.ToSKColor();
                    paint.StrokeWidth = 2;

                    var diameter = Math.Min(info.Width, info.Height) - 4;
                    paint.Style = SKPaintStyle.Stroke;
                    canvas.DrawCircle(info.Width / 2f, info.Height / 2f, diameter / 2f, paint);

                    diameter -= 8;
                    paint.Style = SKPaintStyle.Fill;
                    canvas.DrawCircle(info.Width / 2f, info.Height / 2f, diameter / 2f, paint);
                }

                // we may have been asked to cancel, so jump out
                if (cancellationToken.IsCancellationRequested)
                    return null;

                // HACK: some complex drawing
                canvas.DrawComplexThing();

                // we may have been asked to cancel, so jump out
                if (cancellationToken.IsCancellationRequested)
                    return null;

                // draw the number
                using (var paint = new SKPaint())
                {
                    paint.IsAntialias = true;
                    paint.Style = SKPaintStyle.Fill;
                    paint.Color = SKColors.White;
                    paint.TextSize = info.Height * 0.4f;

                    // measure and center the text, taking into account the
                    // font's paddings and positioning
                    var bounds = new SKRect();
                    paint.MeasureText(data.Number, ref bounds);
                    var pos = new SKPoint(
                        (info.Width - bounds.Width) / 2f - bounds.Left,
                        (info.Height + bounds.Height) / 2f - bounds.Bottom);

                    // draw
                    canvas.DrawText(data.Number, pos, paint);
                }

                // we may have been asked to cancel, so jump out
                if (cancellationToken.IsCancellationRequested)
                    return null;

                // HACK: some complex drawing
                canvas.DrawComplexThing();

                // we may have been asked to cancel, so jump out
                if (cancellationToken.IsCancellationRequested)
                    return null;

                // take a snapshot of the surface, and then clean up
                return surface.Snapshot();
            }
        }
    }

    // HACK: some really, really complex drawing going on in here
    internal static class ComplexDrawingExtensions
    {
        private static readonly Random hackerTimer = new Random();

        public static void DrawComplexThing(this SKCanvas canvas)
        {
            Thread.Sleep(hackerTimer.Next(250));
        }
    }
}
