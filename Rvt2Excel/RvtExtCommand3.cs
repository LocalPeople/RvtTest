using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rvt2Excel
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class RvtExtCommand3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            if (string.IsNullOrWhiteSpace(doc.PathName))
            {
                TaskDialog.Show("Revit", "请保存项目后再进行操作");
                return Result.Cancelled;
            }

            IList<Reference> references;
            if (Selector.PickObjects(uiDoc, out references, "请选择构件"))
            {
                return Result.Cancelled;
            }

            ComponentAnalyser.BeamAnalyser beamAnalyser = new ComponentAnalyser.BeamAnalyser(doc.GetElement(references[0]) as FamilyInstance);
            //TaskDialog.Show("Revit", beamAnalyser.GainValidFaces().Count.ToString());
            beamAnalyser.JoinElementsIntersectArea();
            return Result.Succeeded;
        }
    }
}
