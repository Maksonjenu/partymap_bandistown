using System;
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
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using System.Device.Location;
using System.Windows.Forms;
using mapa.Classes;
using System.Net.Http;

using System.Web;
using Newtonsoft.Json;

using mapa.Classes.CodeBeautify;


namespace mapa
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<PointLatLng> areaspots = new List<PointLatLng>();
        List<MapObject> mapObjects = new List<MapObject>();
        List<MapObject> SortedList = new List<MapObject>();

        List<bandistown> band = new List<bandistown>();


        public MainWindow()
        {
            InitializeComponent();
            initMap();
            //radioButCreate.IsChecked = true;
            //createmodecombo.SelectedIndex = 0;
        }

        public void initMap()
        {
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            Map.MapProvider = OpenStreetMapProvider.Instance;
            Map.MinZoom = 2;
            Map.MaxZoom = 17;
            Map.Zoom = 15;
            Map.Position = new PointLatLng(55.012823, 82.950359);
            Map.MouseWheelZoomType = MouseWheelZoomType.MousePositionWithoutCenter;
            Map.CanDragMap = true;
            Map.DragButton = MouseButton.Left;

        }

        
      

        //private async Task Button_ClickAsync(object sender, RoutedEventArgs e)
        //{
        //    HttpClient httpClient = new HttpClient();
        //    string request = "https://rest.bandsintown.com/v4/artists/maroon%205/events/?app_id=33ef8f94b2739a88ee6db22fa3ced553";
        //    HttpResponseMessage response =
        //        (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();
        //    string responseBody = await response.Content.ReadAsStringAsync();
        //    band.Add (new bandistown(responseBody));
        //}
    


    //public void createMarker(List<PointLatLng> points, int index) 
    //{
    //    MapObject mapObject = null;
    //    switch (index)
    //    {
    //        case 0:
    //            {
    //                mapObject = new Area_class(createObjectName.Text, points);
    //                break;
    //            }
    //        case 1:
    //            {
    //                mapObject = new Location_class(createObjectName.Text, points.Last());
    //                break;
    //            }
    //        case 2:
    //            {
    //                mapObject = new Car_class(createObjectName.Text, points.Last());
    //                break;
    //            }
    //        case 3:
    //            {
    //                mapObject = new Human_class(createObjectName.Text, points.Last());
    //                break;
    //            }
    //        case 4:
    //            {
    //                mapObject = new Route_class(createObjectName.Text, points);
    //                break;
    //            }

    //    }
    //    if (mapObject != null)
    //    {
    //        mapObjects.Add(mapObject);
    //        Map.Markers.Add(mapObject.GetMarker());
    //    }
    //}



    private void MapLoaded(object sender, RoutedEventArgs e)
        {

        }

      



       

        public List<Artists[]> artists_list = new List<Artists[]>();

        async Task<string> sssAsync ()
        {
            HttpClient httpClient = new HttpClient();
            string request = "https://rest.bandsintown.com/v4/artists/"+ tb_name_art.Text +"/events/?app_id=33ef8f94b2739a88ee6db22fa3ced553";
            HttpResponseMessage response =
                (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            var temp = Artists.FromJson(responseBody);  // - эта сука работает остальное нет 

            if (responseBody != "\n\n[]\n")
            {

                artists_list.Add(temp);

                //tb2.Text = ochko.Last()[0].Venue.City;


            }

            else
            {
                System.Windows.MessageBox.Show("Концерты исполнителя не найдены");
            };

                  

            //bandistown test = new bandistown(tb11.Text);

            //test.govno(tb11.Text);

            //var jopa = JsonConvert.DeserializeObject<bandistown>(tb11.Text);

            //band.Add(new bandistown(tb11.Text));

            //tb11.Text = responseBody;

            return responseBody;
            
        }

        private async void btn1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await sssAsync();

                if (artists_list.Count != 0)
                {
                    foreach (Artists i in artists_list.Last())
                    {
                        string lat_str = i.Venue.Latitude.Replace(".", ",");

                        string lng_str = i.Venue.Longitude.Replace(".", ",");

                        double lat = Convert.ToDouble(lat_str);
                        double lng = Convert.ToDouble(lng_str);

                        MapObject mapObject = new Class1(new PointLatLng(lat, lng), i.Venue.Location);
                        mapObjects.Add(mapObject);
                        Map.Markers.Add(mapObject.GetMarker());
                    }

                    listbox_artists.Items.Add(artists_list.Last()[0].Artist.Name);

                }
            }
            catch (System.Net.Http.HttpRequestException)
            {
                System.Windows.MessageBox.Show("Исполнитель не найден");
            }

           
        }

      

     
        

        private void tb_name_art_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            tb_name_art.Text = "";
        }

        private void listbox_artists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            party_list.Items.Clear();
            foreach (Artists ar in artists_list[listbox_artists.SelectedIndex])
            party_list.Items.Add(ar.Venue.Country + " " + ar.Venue.City + " " + ar.Venue.Name);
        }

        private void party_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            if (party_list.SelectedIndex != -1)
                System.Windows.MessageBox.Show(((artists_list[listbox_artists.SelectedIndex][party_list.SelectedIndex]).Datetime).ToString()); // Map.Position 
        }














        //private void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    findsresult.Items.Clear();
        //    PointLatLng clickedPoint = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
        //    SortedList = mapObjects.OrderBy(o => o.getDist(clickedPoint)).ToList();
        //    foreach (MapObject obj in SortedList)
        //    {
        //        string mapObjectAndDistanceString = new StringBuilder()
        //            .Append(obj.getTitle())
        //            .Append(" - ")
        //            .Append(obj.getDist(clickedPoint).ToString("0.##"))
        //            .Append(" м.").ToString();
        //        findsresult.Items.Add(mapObjectAndDistanceString);
        //    }


        //}

        //private void radioButCreate_Checked(object sender, RoutedEventArgs e)
        //{
        //    createmodecombo.IsEnabled = true;
        //    addbuttoncreate.IsEnabled = true;
        //    resetpointcreate.IsEnabled = true;
        //}

        //private void radioButSearch_Checked(object sender, RoutedEventArgs e)
        //{
        //    createmodecombo.IsEnabled = false;
        //    addbuttoncreate.IsEnabled = false;
        //    resetpointcreate.IsEnabled = false;
        //}

        //private void addbuttoncreate_Click(object sender, RoutedEventArgs e)
        //{

        //    if (createObjectName.Text == "")
        //        System.Windows.MessageBox.Show("Имя объекта пустое");
        //    else
        //    {
        //        createMarker(areaspots, createmodecombo.SelectedIndex);
        //        areaspots = new List<PointLatLng>();
        //        createObjectName.Clear();
        //    }
        //    checker();

        //}

        //private void Map_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    double minimum = new double();
        //    PointLatLng clickedPoint = Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y);
        //    MapObject @object = null;
        //    if (mapObjects.Count != 0)
        //    minimum = mapObjects[0].getDist(clickedPoint);
        //    foreach (MapObject obj in mapObjects)
        //    {
        //        if (minimum > obj.getDist(clickedPoint))
        //        {
        //            minimum = obj.getDist(clickedPoint);
        //            @object = obj;
        //        }
        //        if (@object == null)
        //            @object = mapObjects[0];
        //    }

        //    if (@object!=null)
        //    distanceToPoints.Content = $"{Math.Round(minimum)} { @object.getTitle()}"; 

        //}

        //void checker()
        //{
        //    switch (createmodecombo.SelectedIndex)
        //    {
        //        case 0:
        //            {
        //                if (areaspots.Count > 2)
        //                    addbuttoncreate.IsEnabled = true;
        //                else
        //                    addbuttoncreate.IsEnabled = false;
        //                break;
        //            }
        //        case 4:
        //            {
        //                if (areaspots.Count > 1)
        //                    addbuttoncreate.IsEnabled = true;
        //                else
        //                    addbuttoncreate.IsEnabled = false;
        //                break;
        //            }
        //        default :
        //            {
        //                addbuttoncreate.IsEnabled = true;
        //                break;
        //            }

        //    }

        //    if (areaspots.Count == 0)
        //        addbuttoncreate.IsEnabled = false;
        //}

        //private void Map_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    areaspots.Add(Map.FromLocalToLatLng((int)e.GetPosition(Map).X, (int)e.GetPosition(Map).Y));
        //    checker();
        //    clickinfoY.Content = areaspots.Last().Lng;
        //    clickinfoX.Content = areaspots.Last().Lat;
        //}

        //private void createmodecombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    checker();
        //}

        //private void findsresult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{


        //        if (SortedList.Count != 0 && findsresult.SelectedIndex != -1)
        //            Map.Position = SortedList[findsresult.SelectedIndex].getFocus();

        //}




        //private void SearchButton_Click(object sender, RoutedEventArgs e)
        //{
        //    findsresult.Items.Clear();
        //    for (int i = 0; i < mapObjects.Count; i++)
        //        if (mapObjects[i].objectName.Contains(whatineedtofound.Text))
        //        {
        //            findsresult.Items.Add(mapObjects[i].objectName);
        //            SortedList.Add(mapObjects[i]);
        //        }
        //}

        //private void resetpointcreate_Click(object sender, RoutedEventArgs e)
        //{
        //    areaspots = new List<PointLatLng>();
        //    clickinfoX.Content = "0";
        //    clickinfoY.Content = "0";

        //}
    }
    
}




//public ref GMapMarker findRef()
//{
//    return ref new GMapMarker(new PointLatLng());
//}

//PointLatLng point = new PointLatLng(55.016511, 82.946152);

//GMapMarker marker = new GMapMarker(point)
//{
//    Shape = new Image
//    {
//        Width = 32, // ширина маркера
//        Height = 32, // высота маркера
//        ToolTip = "Honda CR-V", // всплывающая подсказка
//        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/men.png")) // картинка
//    }
//};
//Map.Markers.Add(marker);

//GMapMarker marker = new GMapMarker(point)
//{
//    Shape = new Image
//    {
//        Width = 32, // ширина маркера
//        Height = 32, // высота маркера
//        ToolTip = "timetime", // всплывающая подсказка
//        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/notMainSpot.png")) // картинка
//    }
//};





//if (Map.Markers.Count == 0)
//{
//    Map.Markers.Add(marker);
//    clickinfoX.Content = point.Lat;
//    clickinfoY.Content = point.Lng;

//}
//else
//{

//    Map.Markers.Add(marker);     // после выхода из метода ссылка обнуляется 
//    clickinfoX.Content = point.Lat;
//    clickinfoY.Content = point.Lng;
//}