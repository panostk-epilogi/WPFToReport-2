using System;
using System.IO;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using rc = Stimulsoft.Report.Components;
using rd = Stimulsoft.Base.Drawing;


namespace WPFToReport_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DataTable dt = new DataTable();
        private Encoding enc = Encoding.GetEncoding(737);
        private FileStream fStream;
        private string formType;
        private string prFile = "../../737test.txt";
        private Stimulsoft.Report.StiReport rep;
        private Stimulsoft.Report.Components.StiComponentsCollection Coll;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            grd.MouseRightButtonDown += Grd_MouseRightButtonDown;
            TxtToByteArray(prFile);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            rep = new Stimulsoft.Report.StiReport();
            rDesigner.ShowRibbonStatusBar = false;
            rDesigner.OpenReport(rep);
            Coll = rep.Pages[0].Components;
            Coll.ComponentRemoved += Coll_ComponentRemoved;
            
            //throw new NotImplementedException();
        }



        private void Coll_ComponentRemoved(object sender, EventArgs e)
        {
            rc.StiText t = (rc.StiText)sender;

            int i = 0;
//            throw new NotImplementedException();
        }

        private void Grd_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (grd.SelectedCells.Count != 0)
            {
                DataRowView drv = (DataRowView)grd.SelectedCells.First().Item;

                string t = grd.SelectedCells.First().Column.ToString();
                
                Tuple<int, int, int, int> y = GetRegion(grd);
                rc.StiText tx = new rc.StiText(new rd.RectangleD(1, 1, 2, 1), "(" + y.Item1.ToString() + ", " + y.Item2.ToString() + ")-(" + y.Item3.ToString() + ", " + y.Item4.ToString() + ")");

                rep.Pages[0].Components.Add(tx);
                rDesigner.Refresh();

                
                
            }
            //throw new NotImplementedException();
        }

        private Tuple<int, int , int , int> GetRegion (DataGrid myGrid)
        {
            IList<DataGridCellInfo> SelList = (IList<DataGridCellInfo>)myGrid.SelectedCells;
            

            int x1 = int.MaxValue;  
            int x2 = 0;             
            int y1 = int.MaxValue; 
            int y2 = 0; 

            foreach (DataGridCellInfo dci in SelList)
            {
                if (myGrid.Items.IndexOf(dci.Item) > x2)
                    x2 = myGrid.Items.IndexOf(dci.Item);
                if (myGrid.Items.IndexOf(dci.Item) < x1)
                    x1 = myGrid.Items.IndexOf(dci.Item);
                if (dci.Column.DisplayIndex > y2)
                    y2 = dci.Column.DisplayIndex;
                if (dci.Column.DisplayIndex < y1)
                    y1 = dci.Column.DisplayIndex;
                DataGridRow dr = (DataGridRow)myGrid.ItemContainerGenerator.ContainerFromItem(dci.Item);
                DataGridCell dc = (DataGridCell)myGrid.Columns[dci.Column.DisplayIndex].GetCellContent(dr).Parent;
                dc.Background = Brushes.Cyan;
            }


            Tuple<int, int, int, int> y = Tuple.Create(x1, y1, x2, y2);

            return y;
        }



        private void GridInit(List<string> linesList)
        {
           
            grd.HeadersVisibility = DataGridHeadersVisibility.None;
            grd.MaxColumnWidth = 15.0;
            grd.MinColumnWidth = 15.0;
            grd.ItemsSource = dt.DefaultView;
            int maxLength = linesList.
                                OrderByDescending(s => s.Length).
                                FirstOrDefault().Length;

            for (int i = 0; i < maxLength + 1; i++)
            {
                DataColumn dc = new DataColumn("c" + i.ToString("#000", CultureInfo.CurrentCulture), typeof(char));
                dt.Columns.Add(dc);
            }
            for (int i = 0; i < linesList.Count; i++)
            {
                DataRow dr = dt.NewRow();
                dr.ItemArray = linesList[i].
                                    ToCharArray().
                                    Cast<object>().
                                    ToArray();
                dt.Rows.Add(dr);
            }
        }
        private List<string> TxtToByteArray(string pFile)
        {
            
            fStream = File.OpenRead(pFile);
            StreamReader sReader = new StreamReader(fStream, enc);
            formType = sReader.ReadLine();
            List<string> fData = new List<string>();
            
            while (!sReader.EndOfStream)
            {
                string line = sReader.ReadLine();

                fData.Add(line);
            }

            GridInit(fData);

            return fData;
        }
    }
}
