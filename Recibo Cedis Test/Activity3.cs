using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware.Camera2;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using static Android.Widget.AdapterView;

namespace Recibo_Cedis_Test
{
    [Activity(Label = "Activity3")]
    public class Activity3 : Activity
    {
        List<string> detalles = new List<string>();
        List<string> docEntryList = new List<string>();
        List<string> docNumList = new List<string>();
        ListView mainList;

        private ArrayAdapter adp2;
        private SearchView sv2;
        private ListView lv2;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.detalles_Layout);

            mainList = (ListView)FindViewById<ListView>(Resource.Id.listViewDetalles);


            sv2 = FindViewById<SearchView>(Resource.Id.svw2);
            lv2 = FindViewById<ListView>(Resource.Id.listViewDetalles);

            var pendienteSelected = this.Intent.GetStringArrayExtra("Pendientes");
            string[] words;
            int sizeofPendidentes = pendienteSelected.Length;
            string docEntryValues = "";
            for (int j = 0; j < sizeofPendidentes; j++)
            {
                string infoPendientes = pendienteSelected[j];
                words = infoPendientes.Split(" ");

                docEntryValues = words.Last();
                docNumList.Add(words[2]);
                docEntryList.Add(docEntryValues); 
                
            }

            string[] dentry = docEntryList.ToArray();
            string[] docnum = docNumList.ToArray();

            int sizeOfDocEntry = dentry.Length;
            int sizeOfDocNum = docnum.Length;

            for (int i = 0; i < sizeOfDocNum; i++)
            {
                getPendientesDetailData(dentry[i]);

                WebClient client = new WebClient();
                string strPageCode = client.DownloadString("http://192.168.102.79/WebService/jsonFiles/pendientesDetail.json");

                dynamic dobj = JsonConvert.DeserializeObject<dynamic>(strPageCode);

                string sizeObject = getSizeJsonPendientesDetalles();

                int size = Int16.Parse(sizeObject);

                for (int j = 0; j < size; j++)
                {
                    string place = j.ToString();
                    string code = dobj[place]["code"];
                    string desc = dobj[place]["desc"];
                    string qty = dobj[place]["qty"];
                    string umo = dobj[place]["umoCode"];
                    string almacen = dobj[place]["bodega"];
                    string referencia = dobj[place]["ref"];
                    string docNum = docnum[i];

                    string completedInfo = "(" + place + ") " + code + " " + desc + "\n" + "Cant: " + qty + " " + umo + " Alm: " + almacen + " Ref: " + referencia + " Ent= " + docNum;

                    try
                    {
                        detalles.Add(completedInfo);
                    }
                    catch (NullReferenceException e)
                    {
                        Console.WriteLine("ERROR: PLACE - " + place);
                    }
                }
            }

            string[] infoArray = detalles.ToArray(); // array with all the pendientes already structured
                                                     //mainList = (ListView)FindViewById<ListView>(Resource.Id.listView1);
                                                     //mainList.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, infoArray);

            adp2 = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, infoArray);
            lv2.Adapter = adp2;

            /*   lv2.ItemClick += (s, e) => {

                   Dialog dialog = new Dialog(this);
                   dialog.SetContentView(Resource.Layout.liteWarning);
                   TextView dialogText = (TextView)dialog.FindViewById(Resource.Id.textViewProducto);

                   var position = e.Position;

                   dialogText.Text = ;
                   //alert.Create().Show();
                   dialog.Show();

               };*/

            lv2.ItemClick += mylistClick_ItemClick;

            sv2.QueryTextChange += Sv2_QueryTextChange;

        }

        private string getSizeJsonPendientesDetalles()
        {
            WebClient clients = new WebClient();
            string strPageCode = clients.DownloadString("http://192.168.102.79/WebService/jsonFiles/pendientesDetailSize.json");

            dynamic dobj = JsonConvert.DeserializeObject<dynamic>(strPageCode);

            string size = dobj["0"]["size"];

            return size;
        }

        public void getPendientesDetailData(string docEntry)
        {
            string completeUrl = "http://192.168.102.79/WebService/recibosCedisDetail.php?proveedorDocEntry=" + docEntry;
            WebClient client = new WebClient();
            string pageRequest = client.DownloadString(completeUrl);
        }

        private void Sv2_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
        {
            adp2.Filter.InvokeFilter(e.NewText);
        }

        void mylistClick_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string valueList = detalles[e.Position];

            string fixedString = valueList.Substring(0, valueList.LastIndexOf("Cant:"));

            string[] tempString = fixedString.Split(' ');

            string product = tempString.Last();

            Dialog dialog = new Dialog(this);
            dialog.SetContentView(Resource.Layout.liteWarning);
            TextView dialogText = (TextView)dialog.FindViewById(Resource.Id.textViewProducto);
            EditText cantInput = (EditText)dialog.FindViewById(Resource.Id.editTextInputRecibida);
            Button textInput = (Button)dialog.FindViewById(Resource.Id.buttonGuardar);

            dialogText.Text = product;

            //alert.Create().Show();
            dialog.Show();

            textInput.Click += (object sender, EventArgs e) =>
            {
                Console.WriteLine("TACOS");
            };
        }
    }
}