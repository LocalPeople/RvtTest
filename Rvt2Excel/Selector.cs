using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rvt2Excel
{
    static class Selector
    {
        public static bool PickObject(UIDocument uiDoc, out Reference reference, string tip)
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

        public static bool PickObjects(UIDocument uiDoc, out IList<Reference> references, string tip)
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
