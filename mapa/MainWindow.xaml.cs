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




namespace mapa // сделати список мап обжектов в виде массива, что бы отключать маркеры выбранного исполнителя
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<MapObject> mapObjects = new List<MapObject>();

        public bool bool_first = true;

        public List<GeoClass.Artists[]> artists_list = new List<GeoClass.Artists[]>();

        public List<string> art_pars_list = new List<string>()
        {
          "Gordon Rocker" , "Maria Moon" ,  "RYDYR" , "Maroon 5" , "Gorillaz" , "Lil pump" , "Wildways", "Harry" , "Bend" , "Gary lucas" , "Max Leone"
        };


        async Task getResponseAsync(string artName)
        {
            //---------------------
            HttpClient httpClient = new HttpClient();
            string request = "https://rest.bandsintown.com/v4/artists/" + artName + "/events/?app_id=33ef8f94b2739a88ee6db22fa3ced553";
            HttpResponseMessage response =
                (await httpClient.GetAsync(request)).EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            //---------------------


            GeoClass.Artists[] artistsFromResponce = GeoClass.Artists.FromJson(responseBody);
            List<GeoClass.Artists> artistsWithoutSTREAM = new List<GeoClass.Artists>();

            foreach (GeoClass.Artists p in artistsFromResponce)
            {
                if (p.Venue.Location != "")
                {
                    //------------------------------
                    artistsWithoutSTREAM.Add(p);

                   
                    //------------------------------
                }

               
            }

            GeoClass.Artists[] artists = artistsWithoutSTREAM.ToArray<GeoClass.Artists>();

            if (artists.Count() != 0)
            {
                if ((bool_first == true) && (responseBody != "\n\n[]\n"))
                {
                    artists_list.Add(artists);
                    listbox_artists.Items.Add(artists[0].Lineup[0]);
                    bool_first = false;
                    if (artists_list.Count != 0)
                    {
                        foreach (GeoClass.Artists i in artists_list.Last())
                        {
                            string lat_str = i.Venue.Latitude.Replace(".", ",");
                            string lng_str = i.Venue.Longitude.Replace(".", ",");

                            double lat = Convert.ToDouble(lat_str);
                            double lng = Convert.ToDouble(lng_str);

                            MapObject mapObject = new GeoClass(new PointLatLng(lat, lng), i.Venue.Location);
                            mapObjects.Add(mapObject);
                            Map.Markers.Add(mapObject.GetMarker());
                        }

                    }
                }

                else

                            if ((responseBody != "\n\n[]\n") && (artists[0].ArtistId != artists_list.Last()[0].ArtistId) && (bool_first == false)) // брать первый элемент
                {
                    artists_list.Add(artists);
                    listbox_artists.Items.Add(artists[0].Lineup[0]);
                    if (artists_list.Count != 0)
                    {
                        foreach (GeoClass.Artists i in artists_list.Last())
                        {

                            string lat_str = i.Venue.Latitude.Replace(".", ",");
                            string lng_str = i.Venue.Longitude.Replace(".", ",");

                            double lat = Convert.ToDouble(lat_str);
                            double lng = Convert.ToDouble(lng_str);

                            MapObject mapObject = new GeoClass(new PointLatLng(lat, lng), i.Venue.Location);
                            mapObjects.Add(mapObject);
                            Map.Markers.Add(mapObject.GetMarker());

                        }

                    }
                }
                
            }
            else
            {
                lab_art_id.Content = ("Концерты " + artName + " не найдены");
            };
        }


        public MainWindow()
        {
            InitializeComponent();
            initMap();
            pb.Maximum = art_pars_list.Count;

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

        
      


    private void MapLoaded(object sender, RoutedEventArgs e)
        {

        }

        private async void btn1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await getResponseAsync(tb_name_art.Text);

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
            foreach (GeoClass.Artists ar in artists_list[listbox_artists.SelectedIndex])
            party_list.Items.Add(ar.Venue.Country + " " + ar.Venue.City + " " + ar.Venue.Name);

            debug.Text = listbox_artists.SelectedIndex.ToString();
        }

        private void party_list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (party_list.SelectedIndex != -1)
            {
                debug.Text += party_list.SelectedIndex.ToString();

                debug.Text += (party_list.SelectedIndex + listbox_artists.SelectedIndex + listbox_artists.SelectedIndex + 1).ToString();

                int index = 0;

                for (int i = 0; i < listbox_artists.SelectedIndex; i ++)
                {
                    index = index + artists_list[i].Count();
                }
                index = index + party_list.SelectedIndex;
                Map.Position = mapObjects[index].getFocus();   
                
                lab_art_date.Content = "Дата проведения: " + (((artists_list[listbox_artists.SelectedIndex][party_list.SelectedIndex]).Datetime).ToString()); // Map.Position 
                lab_art_id.Content = "Art id - " + (artists_list[listbox_artists.SelectedIndex][0]).ArtistId.ToString();
                lab_art_counrty.Content = "Страна мероприятия: " + (artists_list[listbox_artists.SelectedIndex][party_list.SelectedIndex]).Venue.Country.ToString();
                lab_art_city.Content = "Город мероприятия: " + (artists_list[listbox_artists.SelectedIndex][party_list.SelectedIndex]).Venue.City.ToString();
                lab_art_name.Content = "Название артиста: " + (artists_list[listbox_artists.SelectedIndex][0]).Lineup[0].ToString();
                lab_art_place.Content = "Название площадки: " + (artists_list[listbox_artists.SelectedIndex][party_list.SelectedIndex]).Venue.Name.ToString();
            }
        }




        private async void parse_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                foreach (string name in art_pars_list)
                await getResponseAsync(name);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                System.Windows.MessageBox.Show("Исполнитель не найден");
            }

        }


        



        