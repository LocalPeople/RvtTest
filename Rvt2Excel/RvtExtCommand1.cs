using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rvt2Excel
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class RvtExtCommand1 : IExternalCommand
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
            if (PickObjects(uiDoc, out references, "请选择构件"))
            {
                return Result.Cancelled;
            }

            CellModel[] allCells = new CellModel[references.Count];
            int count = 0;
            foreach (var r in references)
            {
                Element elem = doc.GetElement(r);
                string[] data = elem.LookupParameter("族与类型").AsValueString().Split(':');
                allCells[count++] = new CellModel
                {
                    Id = elem.Id.ToString(),
                    Family = data[0],
                    FamilySymbol = data.Length >= 1 ? data[1] : string.Empty
                };
            }
            Array.Sort(allCells);

            string excelModelPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "族与族类型.xlsx");
            string excelCopyPath = Path.ChangeExtension(doc.PathName, "symbol.xlsx");
            File.Copy(excelModelPath, excelCopyPath, true);

            using (SpreadsheetDocument excel = SpreadsheetDocument.Open(excelCopyPath, true))
            {
                SharedStringTablePart shareStringPart;
                if (excel.WorkbookPart.GetPartsOfType<SharedStringTablePart>().Count() > 0)
                {
                    shareStringPart = excel.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
                }
                else
                {
                    shareStringPart = excel.WorkbookPart.AddNewPart<SharedStringTablePart>();
                }

                WorksheetPart wsPart = excel.WorkbookPart.GetPartById(
                    excel.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().First().Id.Value) as WorksheetPart;
                InsertCellsInTurn(wsPart, shareStringPart, allCells);
                wsPart.Worksheet.Save();
            }

            TaskDialog.Show("XingCengTools", "构件表单已成功导出至项目文档所在目录\n" + excelCopyPath);
            return Result.Succeeded;
        }

        private CacheQueue<string, int> cacheQueue;
        private int InsertSharedStringItem(string text, SharedStringTablePart shareStringPart)
        {
            if (cacheQueue == null)
            {
                cacheQueue = new CacheQueue<string, int>(5);
            }
            int i;
            if (cacheQueue.Contains(text, out i))
            {
                return i;
            }
            i = 0;
            foreach (SharedStringItem item in shareStringPart.SharedStringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == text)
                {
                    cacheQueue.Cache(text, i);
                    return i;
                }

                i++;
            }
            shareStringPart.SharedStringTable.AppendChild(new SharedStringItem(new Text(text)));
            shareStringPart.SharedStringTable.Save();
            cacheQueue.Cache(text, i);
            return i;
        }

        private Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            Row row;
            if (sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).Count() != 0)
            {
                row = sheetData.Elements<Row>().Where(r => r.RowIndex == rowIndex).First();
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            if (row.Elements<Cell>().Where(c => c.CellReference.Value == columnName + rowIndex).Count() > 0)
            {
                return row.Elements<Cell>().Where(c => c.CellReference.Value == cellReference).First();
            }
            else
            {
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, true) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                return newCell;
            }
        }

        private void InsertCellsInTurn(WorksheetPart wsPart, SharedStringTablePart shareStringPart, CellModel[] cellModels)
        {
            Worksheet worksheet = wsPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();

            uint index = 2;
            foreach (var item in cellModels)
            {
                Row row = new Row() { RowIndex = index };
                sheetData.Append(row);

                Cell newCell = new Cell()
                {
                    CellReference = "A" + index,
                    CellValue = new CellValue(item.Id),
                };
                row.Append(newCell);

                newCell = new Cell()
                {
                    CellReference = "B" + index,
                    CellValue = new CellValue(InsertSharedStringItem(item.Family, shareStringPart).ToString()),
                    DataType = new EnumValue<CellValues>(CellValues.SharedString),
                };
                row.Append(newCell);

                newCell = new Cell()
                {
                    CellReference = "C" + index,
                    CellValue = new CellValue(InsertSharedStringItem(item.FamilySymbol, shareStringPart).ToString()),
                    DataType = new EnumValue<CellValues>(CellValues.SharedString),
                };
                row.Append(newCell);

                index++;
            }
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
    }
}
