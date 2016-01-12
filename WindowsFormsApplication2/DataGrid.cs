using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BoxMistr
{
    public class MyDataGridView : DataGridView
    {
        public MyDataGridView()
        {
            // и устанавливаем значение true при создании экземпляра класса
            this.DoubleBuffered = true;
            // или с помощью метода SetStyle
            this.SetStyle(ControlStyles.DoubleBuffer |   ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
        }
    }
}
