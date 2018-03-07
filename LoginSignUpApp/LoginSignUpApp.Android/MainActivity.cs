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
using System.Security.Cryptography;

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

            logIn.Click += async (s,e) =>
            {
                
                Person p = new Person();
                string uName = userName.Text;
                string pWord = userPword.Text;

                var response = await RunGetAsync(uName);

                p.id = response.id;
                p.name = response.name;
                p.password = response.password;
                p.email = response.email;


                string orig = pWord;

                byte[] hashBytes = Convert.FromBase64String(p.password);

                byte[] salt = new byte[16];

                Array.Copy(hashBytes, 0, salt, 0, 16);

                var pbkdf2 = new Rfc2898DeriveBytes(orig, salt, 10000);

                byte[] hash = pbkdf2.GetBytes(20);

                bool ok = true;

                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        ok = false;
                    }
                }

                Console.WriteLine(ok);

                if (ok == true)
                {
                    Intent intent = new Intent(this, typeof(LoggedInActivity));
                    this.StartActivity(intent);
                }
                else
                {

                    Android.App.AlertDialog.Builder message = new Android.App.AlertDialog.Builder(this);

                    message.SetTitle("Opssss");
                    message.SetMessage("The password incorrect!");
                    message.SetNegativeButton("OK", (c, ev) => { });
                    message.Show(); 
                }
            };

            signUp.Click += delegate
            {
                Intent intent = new Intent(this, typeof(SignUpActivity));
                this.StartActivity(intent);
            };
            forgetPword.Click += delegate
            {
                Intent intent = new Intent(this, typeof(ResetPasswordActivity));
                this.StartActivity(intent);
            };


        }

        public async Task<Person> RunGetAsync(string name)
        {
            try
            {
                var RestUrl = "http://10.20.1.68:45455/api/Person/" + name;

                var request = new HttpRequestMessage(HttpMethod.Get, RestUrl);

                var p = new Person();

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    p = JsonConvert.DeserializeObject<Person>(content);
                }
                return p;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}


