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