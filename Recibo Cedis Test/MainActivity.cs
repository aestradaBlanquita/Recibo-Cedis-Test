using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;

namespace Recibo_Cedis_Test
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var qImageView = FindViewById<ImageView>(Resource.Id.imageView1);


            qImageView.SetImageResource(Resource.Drawable.logo);

            var textProveedor = FindViewById<EditText>(Resource.Id.editText1);

            Button buttonBusqueda = FindViewById<Button>(Resource.Id.button1);

            buttonBusqueda.Click += delegate
            {
                var intent = new Intent(this, typeof(Activity2));
                intent.PutExtra("MyData", textProveedor.Text);
                StartActivity(intent);
            };


        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}