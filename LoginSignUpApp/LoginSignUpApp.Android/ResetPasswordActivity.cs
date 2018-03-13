using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace LoginSignUpApp.Droid
{
    [Activity(Label = "ResetPasswordActivity")]
    public class ResetPasswordActivity : Activity
    {
        HttpClient client = new HttpClient();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ResetPassword);

            var codeResponse = "";
            string uName = "";
            string eMail = "";
            EditText uname = FindViewById<EditText>(Resource.Id.uname);
            EditText email = FindViewById<EditText>(Resource.Id.email);
            Button reset = FindViewById<Button>(Resource.Id.resetBtn);

            LinearLayout identLayout = FindViewById<LinearLayout>(Resource.Id.reSetPwordLayout);

            EditText code = FindViewById<EditText>(Resource.Id.newPword);
            //EditText reNewpword = FindViewById<EditText>(Resource.Id.reNewPword);
            Button done = FindViewById<Button>(Resource.Id.doneBtn);
            reset.Click += async (s,e)=>
            {
                Android.App.AlertDialog.Builder message = new Android.App.AlertDialog.Builder(this);

                uName = uname.Text;
                eMail = email.Text;
                //string encoded = "";
                var response = await RunGetAsync(uName);
                if(response != null)
                {
                    Person p = new Person();

                    p.id = response.id;
                    p.name = response.name;
                    p.password = response.password;
                    p.email = response.email;
                    //bool eq = eMail.Equals(p.email);
                    if (eMail.Equals(p.email))
                    {
                        
                        codeResponse = await RunPostAsync(eMail);
                        identLayout.Visibility = ViewStates.Visible;
                        
                        Console.WriteLine(codeResponse);
                    }
                    message.SetTitle("Check your Email");
                    message.SetMessage("We have send a identify code to your Email..");
                    message.SetNegativeButton("OK", (c, ev) => { });
                    message.Show();

                    
                }
                else
                {
                    message.SetTitle("Opssss");
                    message.SetMessage("The user not exist..");
                    message.SetNegativeButton("OK", (c, ev) => { });
                    message.Show();
                }
                
                //Console.WriteLine(eq);
            };

            done.Click += async (s,e) =>
            {
                Android.App.AlertDialog.Builder message = new Android.App.AlertDialog.Builder(this);

                string codeText = code.Text;

                if (codeText.Equals(codeResponse))
                {
                    Intent intent = new Intent(this, typeof(doRestPworActivity));
                    intent.PutExtra("uname", uName);
                    intent.PutExtra("email", eMail);
                    this.StartActivity(intent);
                    Console.WriteLine(codeText.Equals(codeResponse));
                }
                else
                {
                    message.SetTitle("Opssss");
                    message.SetMessage("The code is invalide..");
                    message.SetNegativeButton("OK", (c, ev) => { });
                    message.Show();
                }

                
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

       
        public async Task<string> RunPostAsync(string email)
        {

            try
            {
                HttpClient client = new HttpClient();
                var RestUrl = "http://10.20.1.68:45455/api/Person/" + email;
                var request = new HttpRequestMessage(HttpMethod.Post, RestUrl);

                string password = "";
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    password = JsonConvert.DeserializeObject<string>(content);
                }
                return password;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}