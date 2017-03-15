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

            Element[] elementCollector = new Element[references.Count];
            Dictionary<int, int> idToIndex = new Dictionary<int, int>();
            for (int i = 0; i < elementCollector.Length; i++)
            {
                elementCollector[i] = doc.GetElement(references[i]);
                idToIndex.Add(elementCollector[i].Id.IntegerValue, i);
            }
            int index = 0;
            List<Element[]> joins = new List<Element[]>();
            foreach (var elems in SolidTaskUtil.ParallelSolidFilter(doc, elementCollector))
            {
                foreach (var elem in elems)
                {
                    if (!idToIndex.ContainsKey(elem.Id.IntegerValue) || idToIndex[elem.Id.IntegerValue] > index)
                    {
                        joins.Add(new Element[] { elementCollector[index], elem });
                    }
                }
                index++;
            }

            using (Transaction trans = new Transaction(doc, "Join"))
            {
                trans.Start();
                foreach (var join in joins)
                {
                    JoinGeometryUtils.JoinGeometry(doc, join[0], join[1]);
                }
                trans.Commit();
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
        //Hello world
    }
}
