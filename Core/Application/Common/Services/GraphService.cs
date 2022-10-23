using ImageManipulator.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Pen = System.Drawing.Pen;

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
        /// <remarks>XY axis in Microsoft implementation seems to be inverted when drawing, so for every value i have to subtract height
        /// i.e real 0,0 point is 0,-200. It effectively causes me to begin questioning my sanity at late hours.
        /// </remarks>
        public Bitmap DrawGraphFromInput(Dictionary<Avalonia.Media.Color, double[]> inputData,
            int width = 500,
            int height = 200,
            int horizontalMargins = 5,
            int verticalMargins = 5,
            double brushSize = 1,
            double divideScale = 2)
        {
            var graph = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(graph))
            {
                graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);
                var values = inputData.OrderBy(x => x.Value.Max()).ToArray();

                int maxValue = (int)inputData.Max(x => x.Value.Max());
                int xStartDrawPoint = horizontalMargins;
                int yStartDrawPoint = verticalMargins;

                foreach (KeyValuePair<Avalonia.Media.Color, double[]> graphData in values)
                {
                    using (var pen = new Pen(Color.FromArgb(graphData.Key.R, graphData.Key.G, graphData.Key.B), (float)brushSize))
                    {
                        foreach (int value in graphData.Value)
                        {
                            int reversedHeight = (height - yStartDrawPoint) - (value / (int)divideScale);
                            int reversedHorizontal = width - xStartDrawPoint;

                            graphics.DrawLine(pen, xStartDrawPoint, height - yStartDrawPoint, xStartDrawPoint, reversedHeight);

                            xStartDrawPoint += (int)Math.Ceiling(brushSize);
                        }
                    }
                    xStartDrawPoint = horizontalMargins;
                    yStartDrawPoint = verticalMargins;
                }

                graphics.Save();
            }

            return graph;
        }
    }
}