using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveCard.PdfParser.Models
{
    public struct CropArea
    {
        public int X { get; set; }      // Left
        public int Y { get; set; }      // Top
        public int Width { get; set; }  // Width
        public int Height { get; set; } // Height

        public CropArea(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
