using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedModels.Shapes
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "ShapeType")]
    [JsonDerivedType(typeof(Line), typeDiscriminator: "Line")]
    [JsonDerivedType(typeof(Ellipse), typeDiscriminator: "Ellipse")]
    [JsonDerivedType(typeof(Rectangle), typeDiscriminator: "Rectangle")]
    public abstract class Shape
    {
        [JsonIgnore]
        public string ShapeType { get; set; }
        public string StrokeColor { get; set; }
        public double StrokeThickness { get; set; }

    }
}
