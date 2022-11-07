using Avalonia.Controls.Shapes;
using Avalonia.Media;
using ImageManipulator.Application.Common.Interfaces;
using ImageManipulator.Domain.Common.Dictionaries;
using ImageManipulator.Domain.Common.Helpers;
using ImageManipulator.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageManipulator.Application.Common.Services
{
    public class GraphService : IGraphService
    {
        /// <summary>
        /// Draws new image with histogram
        /// </summary>
        /// <param name="inputData">Color, array of values dictionary</param>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="horizontalMargins">Margin from left corner of image</param>
        /// <param name="verticalMargins">Margin from bottom corner of image</param>
        /// <param name="brushSize">Brush size used in drawline</param>
        /// <param name="divideScale">Division scale for values</param>
        /// <returns>New image with histogram</returns>
        /// <remarks>XY axis in Microsoft implementation seems to be inverted when drawing (hopefuly in WPF is only Y axis inverted!), so for every value i have to subtract height
        /// i.e real 0,0 point is 0,-200. It effectively causes me to begin questioning my sanity at late hours.
        /// </remarks>
        public List<CanvasLineModel> DrawGraphFromInput(Dictionary<string, double[]> inputData,
            int width = 500,
            int height = 200,
            int horizontalMargins = 5,
            int verticalMargins = 5,
            double brushSize = 1,
            double divideScale = 2)
        {
            var lines = new List<CanvasLineModel>(256);
            var values = inputData.OrderBy(x => x.Value.Max()).ToArray();

            int maxValue = (int)inputData.Max(x => x.Value.Max());
            int xStartDrawPoint = horizontalMargins;
            int yStartDrawPoint = verticalMargins;
            int zIndex = 1;

            foreach (KeyValuePair<string, double[]> graphData in values)
            {
                var colour = AvaloniaColourDictionary.Colour[graphData.Key];

                Parallel.ForEach(graphData.Value, value =>
                {
                    int reversedHeight = (int)-(value / (width / height * divideScale));

                    lines.Add(new CanvasLineModel(
                        new Avalonia.Point(xStartDrawPoint, yStartDrawPoint),
                        new Avalonia.Point(xStartDrawPoint + brushSize, reversedHeight),
                        colour,
                        height,
                        zIndex)
                        );

                    xStartDrawPoint += (int)Math.Ceiling(brushSize);
                });

                zIndex++;
                xStartDrawPoint = horizontalMargins;
                yStartDrawPoint = verticalMargins;
            }

            return lines;
        }
    }
}