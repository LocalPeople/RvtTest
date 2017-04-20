using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rvt2Excel.ParamsCopy
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class RvtExtCommand1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            Reference reference;
            if (PickObject(uiDoc, out reference))
                return Result.Cancelled;

            Element elem = doc.GetElement(reference);
            ParamsForm form = new ParamsForm(elem);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                return Result.Cancelled;
            }

            FamilyInstanceFilter filter = new FamilyInstanceFilter(doc, elem.LookupParameter("族").AsElementId());
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            using (Transaction trans = new Transaction(doc, "Copy"))
            {
                trans.Start();
                foreach (var item in collector.WherePasses(filter))
                {
                    item.LookupParameter(form.To).Set(item.LookupParameter(form.From).AsValueString());
                }
                trans.Commit();
            }

            return Result.Succeeded;
        }

        private bool PickObject(UIDocument uiDoc, out Reference reference)
        {
            try
            {
                reference = uiDoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            }
            catch
            {
                reference = null;
            }
            return reference == null;
        }
    }
}
