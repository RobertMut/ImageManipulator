using ImageManipulator.Application.Common.Helpers;
using ImageManipulator.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageManipulator.Application.Common.Services
{
    public class GraphService : IGraphService
    {
        public Avalonia.Media.Imaging.Bitmap DrawGraphFromInput(Dictionary<Color, double[]> inputData,
            int width = 500,
            int height = 200,
            int horizontalMargins = 5,
            int verticalMargins = 5,
            float brushSize = 1.0f,
            double divideScale = 2,
            bool fillImage = true,
            string xLabel = "x",
            string yLabel = "y")
        {
            var graph = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(graph))
            {

                if (fillImage)
                {
                    using (SolidBrush brush = new SolidBrush(Color.White))
                    {
                        graphics.FillRectangle(brush, 0, 0, width, height);
                    }
                }

                DrawLabels(graphics, xLabel, yLabel, 5, horizontalMargins, verticalMargins, brushSize, graph.Width, graph.Height);

                var values = inputData.OrderBy(x => x.Value.Max()).ToArray();

                int maxValue = (int)inputData.Max(x => x.Value.Max());

                int xStartDrawPoint = horizontalMargins;
                int yStartDrawPoint = verticalMargins;
                int colorIterator = 0;

                foreach (KeyValuePair<Color, double[]> graphData in values)
                {
                    using (Pen pen = new Pen(graphData.Key, brushSize))
                    {
                        foreach(int value in graphData.Value)
                        {
                            int reversedHeight = (height - yStartDrawPoint) - (value / (int)divideScale);
                            int reversedHorizontal = width - xStartDrawPoint;

                            graphics.DrawLine(pen, xStartDrawPoint, height-yStartDrawPoint, xStartDrawPoint, reversedHeight);

                            xStartDrawPoint += (int)Math.Ceiling(brushSize);
                        }
                    }

                    xStartDrawPoint = horizontalMargins;
                    yStartDrawPoint = verticalMargins;
                    colorIterator++;
                }

                graphics.Save();
            }

            return ImageConverterHelper.ConvertFromSystemDrawingBitmap(graph);
        }
        
        private Graphics DrawLabels(Graphics graphicsToEdit, string horizontalLabel, string verticalLabel, int fontSize, int horizontalMargin, int verticalMargin, float brushSize, int width, int height)
        {
            using(Pen pen = new Pen(Color.Black, brushSize))
            {
                graphicsToEdit.DrawLine(pen, horizontalMargin, height - verticalMargin, width - horizontalMargin, height - verticalMargin);
                graphicsToEdit.DrawLine(pen, horizontalMargin, height - verticalMargin, horizontalMargin, verticalMargin);
            }

            var font = new Font(FontFamily.GenericSansSerif, fontSize);


            if (horizontalMargin > 5)
            {
                graphicsToEdit.DrawString(horizontalLabel, font, Brushes.Black, (width - horizontalMargin) / 2, height - verticalMargin + 10);
            }
            
            if (verticalMargin > 5)
            {
                graphicsToEdit.DrawString(verticalLabel, font, Brushes.Black,  (height - verticalMargin) / 2, width - horizontalMargin + 10);
            }

            graphicsToEdit.Save();

            return graphicsToEdit;
        }
    }
}