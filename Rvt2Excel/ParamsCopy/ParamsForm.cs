using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace Rvt2Excel.ParamsCopy
{
    public partial class ParamsForm : System.Windows.Forms.Form
    {
        public string From { get { return fromBox.Text; } }
        public string To { get { return toBox.Text; } }

        public ParamsForm()
        {
            InitializeComponent();
        }

        public ParamsForm(Element elem)
        {
            InitializeComponent();

            foreach (Parameter param in elem.Parameters)
            {
                fromBox.Items.Add(param.Definition.Name);
                toBox.Items.Add(param.Definition.Name);
            }
        }
    }
}
