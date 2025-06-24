using System.Windows.Shapes;

namespace Client.Services
{
    public interface ShapeHandler
    {
        Shape CreateWpfShape(SharedModels.Shapes.Shape data);
        SharedModels.Shapes.Shape CreateDataShape(System.Windows.Shapes.Shape wpfShape);
    }
}
