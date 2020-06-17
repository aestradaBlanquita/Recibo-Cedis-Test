using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.Json;
using StructureMap.Pipeline;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Recibo_Cedis_Test
{
    [Activity(Label = "Activity2")]
    public class Activity2 : Activity
    {
        List<string> detalles = new List<string>();
        ListView mainList;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string proveedor = Intent.GetStringExtra("MyData");

            getPendientesData(proveedor);

            SetContentView(Resource.Layout.pendientes_Layout);
            
            WebClient client = new WebClient();
            string strPageCode = client.DownloadString("http://192.168.102.79/WebService/jsonFiles/pendientes.json");

            dynamic dobj = JsonConvert.DeserializeObject<dynamic>(strPageCode);

            string sizeObject = getSizeJsonPendientesa();

            int size = Int16.Parse(sizeObject);

            for (int i = 0; i < size; i++)
            {
                string place = i.ToString();
                string date = dobj[place]["Date"];
                string docNum = dobj[place]["docNum"];
                string proveedorData = dobj[place]["proveedor"];
                string almacen = "3011";
                string docentry = dobj[place]["dentry"];

                string completedInfo = date + " Docnum: " + docNum + " " + "\n" + proveedorData + "\n" + "Almacen: " + almacen + "  " + "DocEntry: " + docentry + "  ";
 
                try
                {
                    detalles.Add(completedInfo);
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine("ERROR: PLACE - " + place);
                }
            }

            string[] infoArray = detalles.ToArray(); // array with all the pendientes already structured
            mainList = (ListView)FindViewById<ListView>(Resource.Id.listView1);
            mainList.Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItemChecked, infoArray);
            mainList.ChoiceMode = ChoiceMode.Multiple;

            /* 
            mainList.ItemClick += (s, e) => {
                var t = infoArray[e.Position];
               // Android.Widget.Toast.MakeText(this, t, Android.Widget.ToastLength.Long).Show();
                var intent = new Intent(this, typeof(Activity3));
                intent.PutExtra("DocEntryValue", t);
                StartActivity(intent);
            };*/
        }

        private string getSizeJsonPendientesa()
        {
            WebClient clients = new WebClient();
            string strPageCode = clients.DownloadString("http://192.168.102.79/WebService/jsonFiles/pendientesSize.json");

            dynamic dobj = JsonConvert.DeserializeObject<dynamic>(strPageCode);

            string size = dobj["0"]["size"];

            return size;
        }

        public void getPendientesData(string proveedor)
        {
            string completeUrl = "http://192.168.102.79/WebService/recibosCedis.php?proveedor=" + proveedor;
            WebClient client = new WebClient();
            string pageRequest = client.DownloadString(completeUrl);
        }
    }
}