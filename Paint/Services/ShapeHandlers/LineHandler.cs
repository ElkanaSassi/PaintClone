using System.Windows.Shapes;

namespace Client.Services.ShapeHandlers
{
    public class LineHandler : ShapeHandler
    {
        public System.Windows.Shapes.Shape CreateWpfShape(SharedModels.Shapes.Shape data)
        {
            var lineData = (SharedModels.Shapes.Line)data;
            return new Line
            {
                X1 = lineData.X1,
                Y1 = lineData.Y1,
                X2 = lineData.X2,
                Y2 = lineData.Y2
            };
        }

        public SharedModels.Shapes.Shape CreateDataShape(System.Windows.Shapes.Shape wpfShape)
        {
            var wpfLine = (Line)wpfShape;
            return new SharedModels.Shapes.Line
            {
                X1 = wpfLine.X1,
                Y1 = wpfLine.Y1,
                X2 = wpfLine.X2,
                Y2 = wpfLine.Y2
            };
        }
    }
}
