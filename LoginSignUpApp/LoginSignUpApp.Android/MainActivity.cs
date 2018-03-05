using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using System.Threading;

namespace LoginSignUpApp.Droid
{
	[Activity (Label = "LoginSignUpApp.Android", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
        HttpClient client = new HttpClient();
        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            EditText userName = FindViewById<EditText>(Resource.Id.uname);
            EditText userPword = FindViewById<EditText>(Resource.Id.pword);

            TextView forgetPword = FindViewById<TextView>(Resource.Id.forgetPword);

            Button logIn = FindViewById<Button>(Resource.Id.loginBtn);
            Button logGoogle = FindViewById<Button>(Resource.Id.logGoogle);
            Button logFacebook = FindViewById<Button>(Resource.Id.logFacebook);

            TextView signUp = FindViewById<TextView>(Resource.Id.signUp);

            logIn.Click += async (s,e)=>
            {

                //var response = await RunPostAsync().ConfigureAwait(false);
                
            };

            signUp.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SignUpActivity));
                this.StartActivity(intent);
            };

		}

        public async Task<Person> RunGetAsync()
        {
            try
            {
                var restURL = "http://192.168.1.75:45455/api/Person/";
                var uri = new Uri(string.Format(restURL, string.Empty));
                var request = new HttpRequestMessage(HttpMethod.Get, restURL);

                Person dd = null;

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dd = JsonConvert.DeserializeObject<Person>(content);
                }
                return dd;
                
            }
            catch(Exception e)
            {
                return null;
            }
        }

        

    }
}


