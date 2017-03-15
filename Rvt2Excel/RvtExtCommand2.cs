using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Rvt2Excel.Parallel;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Rvt2Excel
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class RvtExtCommand2 : IExternalCommand
    {
        private object Lock = new object();

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
            if (PickObjects(uiDoc, out references, "请选择构件"))
            {
                return Result.Cancelled;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            //SolidTaskBucket[] tasks = new SolidTaskBucket[100];
            //for (int i = 0; i < 100; i++)
            //{
            //    GeometryElement geomElement;
            //    Element elem = doc.GetElement(reference);
            //    geomElement = elem.get_Geometry(new Options { ComputeReferences = true });
            //    foreach (var geomObj in geomElement)
            //    {
            //        Solid solid = geomObj as Solid;
            //        if (solid != null && solid.Volume != 0)
            //        {
            //            tasks[i] = new SolidTaskBucket(solid);
            //            tasks[i].SetElementCollector(doc);
            //            tasks[i].Run();
            //        }
            //    }
            //}
            //TaskBucket.WaitAll(tasks);
            //foreach (var collector in tasks)
            //{
            //    count += collector.GetResult().Count;
            //}

            Element[] elementCollector = new Element[references.Count];
            for (int i = 0; i < elementCollector.Length; i++)
            {
                elementCollector[i] = doc.GetElement(references[i]);
            }
            foreach (var elems in SolidTaskUtil.ParallelSolidFilter(doc, elementCollector))
            {
                string dialog = "";
                int index = 1;
                foreach (var elem in elems)
                {
                    dialog += string.Format("{0}: {1}\n", index++, elem.Id);
                }
                TaskDialog.Show("Revit", dialog);
            }

            sw.Stop();
            TaskDialog.Show("Revit", sw.Elapsed.ToString());

            return Result.Succeeded;
        }

        private bool PickObject(UIDocument uiDoc, out Reference reference, string tip)
        {
            try
            {
                reference = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, tip);
            }
            catch
            {
                reference = null;
            }
            return reference == null;
        }

        private bool PickObjects(UIDocument uiDoc, out IList<Reference> references, string tip)
        {
            try
            {
                references = uiDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, tip);
            }
            catch
            {
                references = null;
            }
            return references == null;
        }
    }
}
