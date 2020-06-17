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

namespace Recibo_Cedis_Test
{
    [Activity(Label = "Activity3")]
    public class Activity3 : Activity
    {
        List<string> detalles = new List<string>();
        List<string> docEntryList = new List<string>();
        List<string> docNumList = new List<string>();
        ListView mainList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.detalles_Layout);

            mainList = (ListView)FindViewById<ListView>(Resource.Id.listViewDetalles);

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
            mainList.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, infoArray);

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
    }
}