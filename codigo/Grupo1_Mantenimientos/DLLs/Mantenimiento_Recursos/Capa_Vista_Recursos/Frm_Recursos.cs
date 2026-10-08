using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Capa_Vista_Recursos
{
    public partial class Frm_Recursos : Form
    {
        public Frm_Recursos()
        {
        }

       void navegador1_Load(object sender, EventArgs e)
        {
            navegador1.BotonesEstadoCRUD(true, true, true, true, true);
        }
    }
}
