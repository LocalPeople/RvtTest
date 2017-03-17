using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace Rvt2Excel.ComponentAnalyser
{
    class BeamAnalyser : AnalyserBase
    {
        private GeometryElement beamGeometry;
        private XYZ beamDirection;
        private static readonly double[] rangez = new double[] { -1, -0.707 };

        public BeamAnalyser(FamilyInstance beam) : base(beam)
        {
            beamGeometry = beam.get_Geometry(new Options());
            beamDirection = ((beam.Location as LocationCurve).Curve as Line).Direction;
        }

        public override List<PlanarFace> GainValidFaces()
        {
            List<PlanarFace> faces = new List<PlanarFace>(3);
            foreach (GeometryObject geomObject in beamGeometry)
            {
                using (Solid solid = geomObject as Solid)
                {
                    if (solid != null && solid.Volume > 0)
                    {
                        foreach (Face face in solid.Faces)
                        {
                            if (face is PlanarFace)
                            {
                                PlanarFace pFace = face as PlanarFace;
                                XYZ normal = pFace.ComputeNormal(UV.Zero);
                                if ((normal - beamDirection).GetLength() >= 0.01 && (normal + beamDirection).GetLength() >= 0.01)
                                {
                                    if (Math.Abs(normal.Z) <= 0.01)
                                    {
                                        faces.Add(pFace);
                                    }
                                    else if (normal.Z >= rangez[0] && normal.Z <= rangez[1])
                                    {
                                        faces.Add(pFace);
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }
            return faces;
        }
    }
}
