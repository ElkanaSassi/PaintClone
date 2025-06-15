using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Client.Models
{
    public class ShapeData
    {
        public string ShapeType { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double X2 { get; set; } // for lines cords
        public double Y2 { get; set; } // for lines cords
        public string StrokeColor { get; set; }
        public double StrokeThickness { get; set; }
    }
}
