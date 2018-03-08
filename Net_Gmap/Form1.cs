using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xrm.Client;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Client.Services;
using Microsoft.CSharp;
using System.Data.SqlClient;
using System.Net;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using System.IO;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms.ToolTips;
using System.Globalization;

namespace Net_Gmap
{

    public partial class Form1 : Form
    {
        SqlConnection sqlConnection1;
        SqlCommand cmd = new SqlCommand();
        private QuestEntities3 db = new QuestEntities3();
        private double LATITUDE = 34.13973;
        private double LONGITUDE = -118.03534;
        private GMapOverlay markersOverlay, markersOverlay1;
        private GmapMarkerWithLabel markerLabel = new GmapMarkerWithLabel(new PointLatLng(34.13973, -118.03534), "", GMarkerGoogleType.blue_pushpin);
        private GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(34.13973, -118.03534), GMarkerGoogleType.green);
        private ContextMenu markerMenu = new ContextMenu();
        private MenuItem command1 = null;
        private MenuItem command2 = null;
        private PointLatLng detPoint ;
        MarkerTooltipMode mode;
        Brush ToolTipBackColor = new SolidBrush(Color.Transparent);
        DataSet ds;
        int r=100000;
        SqlDataAdapter sda;
        Timer MyTimer = new Timer();
        double lat;
        double lng;

        private void Method1(object sender, EventArgs e)
        {
            MenuItem item = sender as MenuItem;
            // access item.Tag to get the marker Tag info 
        }
        private void Method2(object sender, EventArgs e)
        {
        }
       

        private void CreateCircle(Double lat, Double lon, double radius,string status, GMapOverlay layer)
        {
            PointLatLng point = new PointLatLng(lat, lon);
            int segments = 1000;

            List<PointLatLng> gpollist = new List<PointLatLng>();

            for (int i = 0; i < segments; i++)
                gpollist.Add(FindPointAtDistanceFrom(point, i, radius / 1000));

            GMapPolygon gpol = new GMapPolygon(gpollist, "pol");

                       

            switch (status.Trim())
            {


                case "N":
                    gpol.Fill = new SolidBrush(Color.FromArgb(70, Color.Red));
                    gpol.Stroke = new Pen(Color.FromArgb(0, Color.Red));
                    break;

                case "n":
                    gpol.Fill = new SolidBrush(Color.FromArgb(70, Color.Red));
                    gpol.Stroke = new Pen(Color.FromArgb(0, Color.Red));
                    break;

                case "Y":
                    gpol.Fill = new SolidBrush(Color.FromArgb(70, Color.Yellow));
                    gpol.Stroke = new Pen(Color.FromArgb(0, Color.Yellow));
                    break;
                case "y":
                    gpol.Fill = new SolidBrush(Color.FromArgb(70, Color.Yellow));
                    gpol.Stroke = new Pen(Color.FromArgb(0, Color.Yellow));
                    break;

                case "S":
                    gpol.Fill = new SolidBrush(Color.FromArgb(70, Color.Green));
                    gpol.Stroke = new Pen(Color.FromArgb(0, Color.Green));
                    break;

                case "s":
                    gpol.Fill = new SolidBrush(Color.FromArgb(70, Color.Green));
                    gpol.Stroke = new Pen(Color.FromArgb(0, Color.Green));
                    break;


                case "no color":
                    gpol.Fill = new SolidBrush(Color.FromArgb(0, Color.Honeydew));
                    gpol.Stroke = new Pen(Color.FromArgb(100, Color.Khaki));
                    break;

                default:
                    gpol.Fill = new SolidBrush(Color.FromArgb(70, Color.Blue));
                    gpol.Stroke = new Pen(Color.FromArgb(0, Color.Blue));
                    break;

            }

            layer.Polygons.Add(gpol);
           
        }

        public static GMap.NET.PointLatLng FindPointAtDistanceFrom(GMap.NET.PointLatLng startPoint, double initialBearingRadians, double distanceKilometres)
        {
            const double radiusEarthKilometres = 6371.01;
            var distRatio = distanceKilometres / radiusEarthKilometres;
            var distRatioSine = Math.Sin(distRatio);
            var distRatioCosine = Math.Cos(distRatio);

            var startLatRad = DegreesToRadians(startPoint.Lat);
            var startLonRad = DegreesToRadians(startPoint.Lng);

            var startLatCos = Math.Cos(startLatRad);
            var startLatSin = Math.Sin(startLatRad);

            var endLatRads = Math.Asin((startLatSin * distRatioCosine) + (startLatCos * distRatioSine * Math.Cos(initialBearingRadians)));

            var endLonRads = startLonRad + Math.Atan2(
                          Math.Sin(initialBearingRadians) * distRatioSine * startLatCos,
                          distRatioCosine - startLatSin * Math.Sin(endLatRads));

            return new GMap.NET.PointLatLng(RadiansToDegrees(endLatRads), RadiansToDegrees(endLonRads));
        }

        public static double DegreesToRadians(double degrees)
        {
            const double degToRadFactor = Math.PI / 180;
            return degrees * degToRadFactor;
        }

        public static double RadiansToDegrees(double radians)
        {
            const double radToDegFactor = 180 / Math.PI;
            return radians * radToDegFactor;
        }


        public Form1()
        {
            InitializeComponent();

            // Initialize map:
            
            gMapControl1.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance;
            gMapControl1.DragButton = MouseButtons.Left;
            GMaps.Instance.Mode = GMap.NET.AccessMode.ServerAndCache;
            markersOverlay = new GMapOverlay("markers");
            markersOverlay1 = new GMapOverlay("markers");
            gMapControl1.Zoom = 2;
            //gMapControl1.SetPositionByKeywords("Irvine, California");
            gMapControl1.Position = new PointLatLng(LATITUDE, LONGITUDE);

            // initialize the commands
            command1 = new MenuItem("Your command name 1", new EventHandler(Method1));
            command2 = new MenuItem("Your command name 2", new EventHandler(Method2));
            markerMenu.MenuItems.Add(command1);
            markerMenu.MenuItems.Add(command2);

        }

        private void MyTimer_Tick(object sender, EventArgs e)
        {

            ds = new DataSet();
            sda = new SqlDataAdapter();
            markersOverlay = new GMapOverlay("markers");

            try
                {
                    sda.SelectCommand = cmd;
                    sda.Fill(ds, "ServiceCalls_table");
                    string address;
                    GeoCoderStatusCode status;
                    PointLatLng myHome = new PointLatLng();
                    //GMarkerGoogleType markType = new GMarkerGoogleType();
                    foreach (DataRow pRow in ds.Tables["ServiceCalls_table"].Rows)
                    {
      //                  address = pRow["Address"].ToString();

      //                  if (!string.IsNullOrEmpty(address))
      //                  {
      //                      try
      //                      {
      //                          myHome = (PointLatLng)GMapProviders.GoogleMap.GetPoint(address, out status);

      //                      }
      //                      catch (Exception ex)
      //                      {
      //                        //  MessageBox.Show("Warning!! CallNo: " + pRow["CallID"] + " address can not be located : " + address + " " + ex.Message.ToString());
      //                          // MessageBox.Show("CallNo: " + pRow["CallID"] + "  " + address + " " + ex.StackTrace.ToString());
      //                      }

      //                  //markersOverlay.Markers.Add(new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new PointLatLng(33.68395, -117.79469), GMarkerGoogleType.blue_pushpin));
      //                  if (myHome != null)
      //                  {
      //                      var rnd = new System.Random();
      //                      var t= new string(Enumerable.Repeat("DFLOPST", 1)
      //.Select(s => s[rnd.Next(s.Length)]).ToArray());
      //                      rnd.Next( );

                            //switch (pRow["ServiceStatus"].ToString())
                            //{

                            //    case "D":
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.blue);

                            //        break;
                            //    case "F":
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.green);

                            //        break;
                            //    case "L":
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.lightblue);

                            //        break;
                            //    case "O":
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.red);

                            //        break;
                            //    case "P":
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.purple);

                            //        break;
                            //    case "S":
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.yellow);

                            //        break;
                            //    case "T":
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.pink);

                            //        break;
                            //    default:
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.blue);

                            //        break;

                            //}



                            //switch (pRow["Confirm"].ToString())
                            //{


                            //    case "":
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.red);

                            //        break;

                            //    case "Y":
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.yellow);

                            //        break;
                            //    case "y":
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.yellow);

                            //        break;

                            //    default:
                            //        marker = new GMarkerGoogle(myHome, GMarkerGoogleType.blue);

                            //        break;

                            //}
                           
                                int size = Convert.ToInt32(pRow["DeviceCount"])>100?100: Convert.ToInt32(pRow["DeviceCount"]);
                          
                           CreateCircle(System.Convert.ToSingle(pRow["Lat"]), System.Convert.ToSingle(pRow["Long"]), size*1000, pRow["Confirm"].ToString(), markersOverlay);

                            AddMarker(System.Convert.ToSingle(pRow["Lat"]), System.Convert.ToSingle(pRow["Long"]), pRow["DeviceCount"].ToString());

                            //marker.ToolTip = new GMapBaloonToolTip(marker);
                            //marker.ToolTipMode = mode;
                            //marker.ToolTipText = "Call# " + pRow["CallID"];
                            //marker.Tag = "Call# " + pRow["CallID"];

                            // markersOverlay.Markers.Add(marker);
                       // }
                   // }
                    }

                gMapControl1.Overlays.Add(markersOverlay);
                gMapControl1.Overlays.Add(markersOverlay1);
            }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                    MessageBox.Show(ex.StackTrace.ToString());

                }

        }

        private void AddMarker(Single a,Single b, string c)
        {
           var  marker1 = new GMarkerCross(new PointLatLng(a, b));
            marker1.ToolTip = new GMapBaloonToolTip(marker1);
            marker1.ToolTipMode = MarkerTooltipMode.Always;
            marker1.ToolTipText = "Call# " + c;
            marker1.Tag = "Call# " + c;
           // marker1.IsVisible = false;

            markersOverlay1.Markers.Add(marker1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection1 = new SqlConnection(@"Server=10.10.1.86;Database=questnet;UID=sa;PWD=s1ungod;packet size=4096");

            //sqlConnection1 = new SqlConnection(@"Server=138.91.249.208,3433;Database=questnet;UID=sa3;PWD=s1ungod;packet size=4096");
           // cmd.CommandText = "SELECT DeviceCount,Confirm,Lat,Long,[Address1] +', '+[City]+', '+ [State] +', '+[Country] AS Address FROM ax_SmithHospitalLocation";
            cmd.CommandText = "SELECT DeviceCount,Confirm,Lat,Long FROM ax_SmithHospitalLocation";
            //sqlConnection1 = new SqlConnection(@"Server=10.10.1.86;Database=questnet;UID=sa3;PWD=s1ungod;packet size=4096");

            //cmd.CommandText = "  SELECT TOP(100) PERCENT dbo.ServiceCalls.callid, dbo.ServiceCalls.ServiceStatus, ";
            //cmd.CommandText += "     dbo.AssetLocation.Address1 + ', ' + dbo.AssetLocation.City + ', ' + dbo.AssetLocation.State + ', ' + dbo.AssetLocation.Zip + ', ' + dbo.AssetLocation.Country AS Address";
            //cmd.CommandText += "  FROM dbo.ServiceCalls INNER JOIN";
            //cmd.CommandText += "     dbo.Locations ON dbo.ServiceCalls.CompanyNo = dbo.Locations.CompanyNo AND dbo.ServiceCalls.SiteCode = dbo.Locations.SiteCode LEFT OUTER JOIN";
            //cmd.CommandText += "     dbo.AssetLocation ON dbo.ServiceCalls.callid = dbo.AssetLocation.CallID";
            //cmd.CommandText += "  WHERE(dbo.ServiceCalls.ServiceStatus <> 'C') AND(dbo.ServiceCalls.ServiceStatus <> 'X') AND(dbo.ServiceCalls.ServiceStatus <> 'E') AND(dbo.ServiceCalls.ServiceStatus <> 'G') AND(dbo.AssetLocation.CallID IS NOT NULL) and(dbo.AssetLocation.Address1 <> null)";
            //cmd.CommandText += "     union";
            //cmd.CommandText += " SELECT        TOP(100) PERCENT dbo.ServiceCalls.callid, dbo.ServiceCalls.ServiceStatus, dbo.Locations.Address1 + ', ' + dbo.Locations.City + ', ' + dbo.Locations.State + ', ' + dbo.Locations.Zip + ', ' + dbo.Locations.Country AS Address";
            //cmd.CommandText += " FROM dbo.ServiceCalls INNER JOIN";
            //cmd.CommandText += "  dbo.Locations ON dbo.ServiceCalls.CompanyNo = dbo.Locations.CompanyNo AND dbo.ServiceCalls.SiteCode = dbo.Locations.SiteCode LEFT OUTER JOIN";
            //cmd.CommandText += "       dbo.AssetLocation ON dbo.ServiceCalls.callid = dbo.AssetLocation.CallID";
            //cmd.CommandText += "  WHERE(dbo.ServiceCalls.ServiceStatus <> 'C') AND(dbo.ServiceCalls.ServiceStatus <> 'X') AND(dbo.ServiceCalls.ServiceStatus <> 'E') AND(dbo.ServiceCalls.ServiceStatus <> 'G') AND(dbo.AssetLocation.CallID IS NULL)";
            //cmd.CommandText += " union ";
            //cmd.CommandText += " SELECT        28000 AS callId, 'O' AS ServiceStatus, Address1 + ', ' + City + ', ' + State + ', ' + Zip + ', ' + Country AS Address ";
            //cmd.CommandText += " FROM            dbo.Locations ";
            //cmd.CommandText += " WHERE        (Mark = N'Y') ";
            //cmd.CommandText += " GROUP BY Address1 + ', ' + City + ', ' + State + ', ' + Zip + ', ' + Country ";
            //cmd.CommandText += " HAVING        (Address1 + ', ' + City + ', ' + State + ', ' + Zip + ', ' + Country IS NOT NULL) ";

            //cmd.CommandText = "  SELECT TOP(100) PERCENT dbo.ServiceCalls.callid, dbo.ServiceCalls.ServiceStatus, ";


            cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@ItemCode", SqlDbType.VarChar);
            //cmd.Parameters["@ItemCode"].Value = itemList[0].ItemCode;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;
            sqlConnection1.Open();

           
            MyTimer.Interval = (5 * 60 * 1000); 
            MyTimer.Tick += new EventHandler(MyTimer_Tick);
            MyTimer.Start();
            MyTimer_Tick(null, EventArgs.Empty);


            //GMarkerGoogle marker = new GMarkerGoogle(new PointLatLng(LATITUDE, LONGITUDE), GMarkerGoogleType.green);

            //markersOverlay.Markers.Add(marker);
            //string address = "65 Parker, Irvine, CA";
            //GeoCoderStatusCode status;
            //PointLatLng myHome = new PointLatLng();
            //myHome = (PointLatLng)GMapProviders.GoogleMap.GetPoint(address, out status);

            //markersOverlay.Markers.Add(new GMap.NET.WindowsForms.Markers.GMarkerGoogle(new PointLatLng(33.68395, -117.79469), GMarkerGoogleType.blue_pushpin));

            //markersOverlay.Markers.Add(new GMap.NET.WindowsForms.Markers.GMarkerGoogle(myHome ,GMarkerGoogleType.blue));
                   
            //PointLatLng start = new PointLatLng(33.68395, -117.79469);
            //PointLatLng end = new PointLatLng(LATITUDE, LONGITUDE);
            try
            {
                //MapRoute route = GMap.NET.MapProviders.OpenStreetMapProvider.Instance.GetRoute(start, end, false, false, 15);

                // GMapRoute r = new GMapRoute(route.Points, "My route");

                //GMapOverlay routesOverLay = new GMapOverlay("rountes");

                //markersOverlay.Routes.Add(r);


              //  gMapControl1.Overlays.Add(markersOverlay);
        //        gMapControl1.Overlays.Add(markersOverlay1);
           //     gMapControl1.MouseClick -= new MouseEventArgs(this.gMapControl1_OnMarkerClick);
            }
            catch (Exception ex)
            {
            }

        }

        private void gMapControl1_OnMarkerClick(GMapMarker item, MouseEventArgs e)
        {
            object identityData = item.Tag;

            //load the menus with marker data.
            command1.Tag = identityData;
            command2.Tag = identityData;
            gMapControl1.Zoom = 18;
            gMapControl1.Position = item.Position;
            marker.ToolTip = new GMapBaloonToolTip(marker);
            marker.ToolTipMode = mode;
            marker.ToolTip.Fill = ToolTipBackColor;
            //if (item.Tag == null)
            //    marker.ToolTipText = "Quest International";
            //else marker.ToolTipText = item.Tag.ToString();
            detPoint = item.Position;
            if (identityData != null && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                markerMenu.Show(gMapControl1, e.Location);
            }
        }

  
        private void gMapControl1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
          
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                lat = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lat;
                lng = gMapControl1.FromLocalToLatLng(e.X, e.Y).Lng;

                marker = new GMarkerGoogle(new PointLatLng(lat, lng), GMarkerGoogleType.red_big_stop);
                markersOverlay1.Clear();
               // gMapControl1.Overlays.Remove(markersOverlay1);
               // markersOverlay1 = new GMapOverlay();
                markersOverlay1.Markers.Add(marker);
                                
                CreateCircle(lat, lng, r, "no color", markersOverlay1);
                gMapControl1.Overlays.Add(markersOverlay1);

                //MapRoute route = GMap.NET.MapProviders.OpenStreetMapProvider.Instance.GetRoute(new PointLatLng(lat, lng), detPoint, false, false, 15);

                // GMapRoute r = new GMapRoute(route.Points, "My route");

                //GMapOverlay routesOverLay = new GMapOverlay("rountes");

                //markersOverlay.Routes.Add(r);

            }
        }

        private void gMapControl1_Load(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

          
            gMapControl1.Overlays.Clear();
           // gMapControl1.Refresh();
            mode = MarkerTooltipMode.OnMouseOver;
            MyTimer_Tick(null, EventArgs.Empty);

            gMapControl1.Overlays.Add(markersOverlay);

           

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            
            gMapControl1.Overlays.Clear();
            mode = MarkerTooltipMode.Always;

            MyTimer_Tick(null, EventArgs.Empty);
            gMapControl1.Overlays.Add(markersOverlay);
        

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
                r = System.Convert.ToInt32(textBox1.Text);
            else
                r = 10000;

      
        }
    }
}
