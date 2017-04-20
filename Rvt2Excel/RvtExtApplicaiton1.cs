using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rvt2Excel
{
    class RvtExtApplicaiton1 : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            List<RibbonPanel> rps = application.GetRibbonPanels(Tab.AddIns);
            RibbonPanel ribbonPanel = rps.FirstOrDefault(rp => rp.Name == "星层工具");
            if (ribbonPanel == null)
            {
                ribbonPanel = application.CreateRibbonPanel("星层工具");
            }

            string dirName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            ribbonPanel.AddItem(new PushButtonData("ButtonExport", "\n\n导出\n构件表单", Path.Combine(dirName, "Rvt2Excel.dll"), "Rvt2Excel.RvtExtCommand1"));
            ribbonPanel.AddItem(new PushButtonData("ButtonCopy", "\n\n复制\n属性", Path.Combine(dirName, "Rvt2Excel.dll"), "Rvt2Excel.ParamsCopy.RvtExtCommand1"));
            ribbonPanel.AddItem(new PushButtonData("ButtonParalJoin", "\n\n自动\n连接构件", Path.Combine(dirName, "Rvt2Excel.dll"), "Rvt2Excel.RvtExtCommand2"));

            return Result.Succeeded;
        }
    }
}
