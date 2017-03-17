using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Rvt2Excel.ComponentAnalyser
{
    abstract class AnalyserBase
    {
        protected Element component;

        public AnalyserBase(Element component)
        {
            this.component = component;
        }

        /// <summary>
        /// 获取构件有效平面的子类重载函数
        /// </summary>
        public abstract List<PlanarFace> GainValidFaces();

        public virtual Dictionary<ElementId, double> JoinElementsIntersectArea()
        {
            //foreach (var pFace in GainValidFaces())
            //{
            //    Transform transform = GetTransform(pFace.Origin, pFace.ComputeNormal(UV.Zero));
            //    TaskDialog.Show("Revit", "New Face !!!");
            //    foreach (var point in pFace.Triangulate().Vertices)
            //    {
            //        TaskDialog.Show("Revit", transform.OfPoint(point).ToString());
            //    }
            //}
            throw new NotImplementedException();
        }

        private Transform GetTransform(XYZ origin, XYZ normal)
        {
            double anglex, anglez;

            if ((normal + XYZ.BasisZ).GetLength() <= 0.01 || (normal - XYZ.BasisZ).GetLength() <= 0.01)
                return Transform.Identity;

            XYZ vector2d = new XYZ(normal.X, normal.Y, 0);
            if (vector2d.Y >= 0)
            {
                if (vector2d.X >= 0)
                {
                    anglez = vector2d.AngleTo(XYZ.BasisY);
                }
                else
                {
                    anglez = -vector2d.AngleTo(XYZ.BasisY);
                }
            }
            else
            {
                if (vector2d.X >= 0)
                {
                    anglez = -vector2d.AngleTo(-XYZ.BasisY);
                }
                else
                {
                    anglez = vector2d.AngleTo(-XYZ.BasisY);
                }
            }

            if (normal.Z > 0)
            {
                if (normal.Y >= 0)
                {
                    anglex = normal.AngleTo(XYZ.BasisZ);
                }
                else
                {
                    anglex = -normal.AngleTo(XYZ.BasisZ);
                }
            }
            else
            {
                if (normal.Y >= 0)
                {
                    anglex = -normal.AngleTo(-XYZ.BasisZ);
                }
                else
                {
                    anglex = normal.AngleTo(-XYZ.BasisZ);
                }
            }

            return Transform.CreateRotationAtPoint(XYZ.BasisX, anglex, origin) * Transform.CreateRotationAtPoint(XYZ.BasisZ, anglez, origin);
        }
    }
}
